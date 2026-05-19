using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models.Dtos.Produtos.Lojas.Entregas;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class LojaEntregaService
    {
        private readonly DataContext _context;

        public LojaEntregaService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<LojaEntregaOpcaoLeituraDto>> ListarMinhasOpcoesAsync(int usuarioId)
        {
            var lojaId = await _context.TBL_LOJA
                .AsNoTracking()
                .Where(l => l.UsuarioId == usuarioId)
                .Select(l => (int?)l.Id)
                .FirstOrDefaultAsync();

            if (!lojaId.HasValue)
                return new List<LojaEntregaOpcaoLeituraDto>();

            var opcoes = await _context.TBL_LOJA_ENTREGA_OPCAO
                .AsNoTracking()
                .Where(o => o.LojaId == lojaId.Value)
                .OrderByDescending(o => o.Ativa)
                .ThenBy(o => o.TipoEntregaId)
                .ThenBy(o => o.Nome)
                .ToListAsync();

            return opcoes.Select(Mapear).ToList();
        }

        public async Task<List<LojaEntregaOpcaoLeituraDto>> ListarOpcoesPublicasAsync(
            int lojaId,
            string? cep,
            string? cidade,
            string? uf)
        {
            var opcoes = await _context.TBL_LOJA_ENTREGA_OPCAO
                .AsNoTracking()
                .Where(o => o.LojaId == lojaId && o.Ativa)
                .OrderBy(o => o.ValorFrete)
                .ThenBy(o => o.PrazoEntregaDias)
                .ToListAsync();

            return opcoes.Select(Mapear).ToList();
        }

        public async Task<LojaEntregaOpcaoLeituraDto> CriarMinhaOpcaoAsync(int usuarioId, LojaEntregaOpcaoCriacaoDto dto)
        {
            var loja = await _context.TBL_LOJA
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);

            if (loja == null)
                throw new InvalidOperationException("Cadastre sua loja antes de configurar entregas.");

            var opcao = new LojaEntregaOpcao
            {
                LojaId = loja.Id,
                DataCriacao = DateTime.UtcNow
            };

            AplicarDados(opcao, dto);

            await _context.TBL_LOJA_ENTREGA_OPCAO.AddAsync(opcao);
            await _context.SaveChangesAsync();

            return Mapear(opcao);
        }

        public async Task<LojaEntregaOpcaoLeituraDto?> AtualizarMinhaOpcaoAsync(
            int usuarioId,
            int opcaoId,
            LojaEntregaOpcaoAtualizacaoDto dto)
        {
            var opcao = await _context.TBL_LOJA_ENTREGA_OPCAO
                .Include(o => o.Loja)
                .FirstOrDefaultAsync(o => o.Id == opcaoId && o.Loja.UsuarioId == usuarioId);

            if (opcao == null)
                return null;

            AplicarDados(opcao, dto);
            opcao.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Mapear(opcao);
        }

        public async Task<bool> RemoverMinhaOpcaoAsync(int usuarioId, int opcaoId)
        {
            var opcao = await _context.TBL_LOJA_ENTREGA_OPCAO
                .Include(o => o.Loja)
                .FirstOrDefaultAsync(o => o.Id == opcaoId && o.Loja.UsuarioId == usuarioId);

            if (opcao == null)
                return false;

            _context.TBL_LOJA_ENTREGA_OPCAO.Remove(opcao);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void AplicarDados(LojaEntregaOpcao opcao, LojaEntregaOpcaoCriacaoDto dto)
        {
            ValidarDto(dto);

            opcao.TipoEntregaId = dto.TipoEntregaId;
            opcao.Nome = dto.Nome.Trim();
            opcao.ValorFrete = EntregaHelper.TipoEntregaEhRetirada(dto.TipoEntregaId) ? 0 : dto.ValorFrete;
            opcao.PrazoEntregaDias = dto.PrazoEntregaDias;
            opcao.Observacao = LimparOpcional(dto.Observacao);
            opcao.Ativa = dto.Ativa;
        }

        private static void AplicarDados(LojaEntregaOpcao opcao, LojaEntregaOpcaoAtualizacaoDto dto)
        {
            AplicarDados(opcao, new LojaEntregaOpcaoCriacaoDto
            {
                TipoEntregaId = dto.TipoEntregaId,
                Nome = dto.Nome,
                ValorFrete = dto.ValorFrete,
                PrazoEntregaDias = dto.PrazoEntregaDias,
                Observacao = dto.Observacao,
                Ativa = dto.Ativa
            });
        }

        private static void ValidarDto(LojaEntregaOpcaoCriacaoDto dto)
        {
            if (!EntregaHelper.TipoEntregaValido(dto.TipoEntregaId))
                throw new InvalidOperationException("Tipo de entrega invalido.");
        }

        private static LojaEntregaOpcaoLeituraDto Mapear(LojaEntregaOpcao opcao)
        {
            return new LojaEntregaOpcaoLeituraDto
            {
                Id = opcao.Id,
                LojaId = opcao.LojaId,
                TipoEntrega = EntregaHelper.ObterNomeTipoEntrega(opcao.TipoEntregaId),
                Nome = opcao.Nome,
                ValorFrete = opcao.ValorFrete,
                PrazoEntregaDias = opcao.PrazoEntregaDias,
                Observacao = opcao.Observacao,
                Ativa = opcao.Ativa,
                ResumoCobertura = "Entrega configurada pela loja",
                DataCriacao = opcao.DataCriacao,
                DataAtualizacao = opcao.DataAtualizacao
            };
        }

        private static string? LimparOpcional(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return valor.Trim();
        }
    }
}
