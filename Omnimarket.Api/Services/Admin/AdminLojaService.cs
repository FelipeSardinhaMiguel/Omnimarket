using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Extensions;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services
{
    public class AdminLojaService
    {
        private readonly DataContext _context;

        public AdminLojaService(DataContext context)
        {
            _context = context;
        }

        public async Task<PageResult<AdminLojaDto>> ListarLojasAsync(
            string? busca,
            bool? ativa,
            int page,
            int pageSize)
        {
            var query = _context.TBL_LOJA
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = $"%{busca.Trim()}%";
                query = query.Where(l =>
                    EF.Functions.Like(l.NomeFantasia, termo) ||
                    EF.Functions.Like(l.Slug, termo) ||
                    (l.Endereco != null && EF.Functions.Like(l.Endereco.Cidade, termo)) ||
                    EF.Functions.Like(l.Usuario.Nome, termo) ||
                    EF.Functions.Like(l.Usuario.Email, termo));
            }

            if (ativa.HasValue)
                query = query.Where(l => l.Ativa == ativa.Value);

            return await query
                .OrderByDescending(l => l.DtCriacao)
                .Select(l => new AdminLojaDto
                {
                    Id = l.Id,
                    UsuarioId = l.UsuarioId,
                    NomeFantasia = l.NomeFantasia,
                    Slug = l.Slug,
                    Cidade = l.Endereco != null ? l.Endereco.Cidade : null,
                    Uf = l.Endereco != null ? l.Endereco.Uf : null,
                    Ativa = l.Ativa,
                    NomeResponsavel = l.Usuario.Nome + " " + l.Usuario.Sobrenome,
                    EmailResponsavel = l.Usuario.Email,
                    TotalProdutos = l.Produtos.Count,
                    ProdutosPublicados = l.Produtos.Count(p => p.StatusPublicacao == StatusProduto.Publicado),
                    DtCriacao = l.DtCriacao,
                    DtAtualizacao = l.DtAtualizacao
                })
                .ToPagedResultAsync(page, pageSize);
        }

        public async Task<AdminLojaDto?> AtualizarStatusLojaAsync(int lojaId, AdminAtualizarStatusLojaDto dto)
        {
            var loja = await _context.TBL_LOJA
                .Include(l => l.Endereco)
                .Include(l => l.Usuario)
                .Include(l => l.Produtos)
                .FirstOrDefaultAsync(l => l.Id == lojaId);

            if (loja == null)
                return null;

            loja.Ativa = dto.Ativa;
            loja.DtAtualizacao = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return loja.ToAdminDto();
        }
    }
}
