using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Usuarios.Enderecos;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class EnderecoService
    {
        private readonly DataContext _context;

        public EnderecoService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<UsuarioEnderecoLeituraDto>> ListarAsync(int usuarioId, bool incluirInativos)
        {
            var query = _context.TBL_ENDERECO
                .Where(e => e.UsuarioId == usuarioId);

            if (!incluirInativos)
                query = query.Where(e => e.Ativo);

            var enderecos = await query
                .OrderByDescending(e => e.IsPrincipal)
                .ThenBy(e => e.Id)
                .ToListAsync();

            return enderecos.Select(MapearEndereco).ToList();
        }

        public async Task<UsuarioEnderecoLeituraDto?> ObterAsync(int usuarioId, int enderecoId, bool incluirInativos)
        {
            var query = _context.TBL_ENDERECO
                .Where(e => e.UsuarioId == usuarioId && e.Id == enderecoId);

            if (!incluirInativos)
                query = query.Where(e => e.Ativo);

            var endereco = await query.FirstOrDefaultAsync();
            return endereco == null ? null : MapearEndereco(endereco);
        }

        public async Task<UsuarioEnderecoLeituraDto> CriarAsync(int usuarioId, UsuarioEnderecoDto dto)
        {
            var usuarioExiste = await _context.TBL_USUARIO.AnyAsync(u => u.Id == usuarioId);
            if (!usuarioExiste)
                throw new KeyNotFoundException("Usuario nao encontrado.");

            var possuiEnderecoAtivo = await _context.TBL_ENDERECO
                .AnyAsync(e => e.UsuarioId == usuarioId && e.Ativo);

            var seraPrincipal = dto.IsPrincipal == true || !possuiEnderecoAtivo;

            if (seraPrincipal)
                await DesmarcarEnderecoPrincipalAsync(usuarioId);

            var endereco = new Endereco
            {
                UsuarioId = usuarioId,
                Cep = dto.Cep.Replace("-", "").Trim(),
                TipoLogradouro = dto.TipoLogradouro,
                NomeEndereco = dto.NomeEndereco.Trim(),
                Numero = dto.Numero.Trim(),
                Complemento = dto.Complemento?.Trim(),
                Cidade = dto.Cidade.Trim(),
                Uf = dto.Uf.Trim().ToUpper(),
                IsPrincipal = seraPrincipal,
                Ativo = true
            };

            await _context.TBL_ENDERECO.AddAsync(endereco);
            await _context.SaveChangesAsync();

            return MapearEndereco(endereco);
        }

        public async Task<UsuarioEnderecoLeituraDto?> AtualizarAsync(int usuarioId, int enderecoId, UsuarioEnderecoDto dto)
        {
            var endereco = await _context.TBL_ENDERECO
                .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId && e.Id == enderecoId && e.Ativo);

            if (endereco == null)
                return null;

            endereco.Cep = dto.Cep.Replace("-", "").Trim();
            endereco.TipoLogradouro = dto.TipoLogradouro;
            endereco.NomeEndereco = dto.NomeEndereco.Trim();
            endereco.Numero = dto.Numero.Trim();
            endereco.Complemento = dto.Complemento?.Trim();
            endereco.Cidade = dto.Cidade.Trim();
            endereco.Uf = dto.Uf.Trim().ToUpper();

            if (dto.IsPrincipal == true)
            {
                await DesmarcarEnderecoPrincipalAsync(usuarioId, enderecoId);
                endereco.IsPrincipal = true;
            }
            else if (dto.IsPrincipal == false && endereco.IsPrincipal)
            {
                endereco.IsPrincipal = false;
                await DefinirNovoPrincipalSeNecessarioAsync(usuarioId, enderecoId);
            }

            await _context.SaveChangesAsync();
            return MapearEndereco(endereco);
        }

        public async Task<bool> RemoverAsync(int usuarioId, int enderecoId)
        {
            var endereco = await _context.TBL_ENDERECO
                .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId && e.Id == enderecoId && e.Ativo);

            if (endereco == null)
                return false;

            var eraPrincipal = endereco.IsPrincipal;

            endereco.Ativo = false;
            endereco.IsPrincipal = false;

            if (eraPrincipal)
                await DefinirNovoPrincipalSeNecessarioAsync(usuarioId, enderecoId);

            await _context.SaveChangesAsync();
            return true;
        }

        public List<TipoLogradouroOpcaoDto> ObterTiposLogradouro()
        {
            return Enum.GetValues<TiposLogradouroBR>()
                .Select(v => new TipoLogradouroOpcaoDto
                {
                    Codigo = v.ToString(),
                    Descricao = EnumExtensions.GetDisplayName(v)
                })
                .ToList();
        }

        private async Task DesmarcarEnderecoPrincipalAsync(int usuarioId, int? enderecoIgnoradoId = null)
        {
            var enderecosPrincipais = await _context.TBL_ENDERECO
                .Where(e => e.UsuarioId == usuarioId && e.Ativo && e.IsPrincipal)
                .Where(e => !enderecoIgnoradoId.HasValue || e.Id != enderecoIgnoradoId.Value)
                .ToListAsync();

            foreach (var endereco in enderecosPrincipais)
                endereco.IsPrincipal = false;
        }

        private async Task DefinirNovoPrincipalSeNecessarioAsync(int usuarioId, int enderecoIgnoradoId)
        {
            var jaExistePrincipal = await _context.TBL_ENDERECO
                .AnyAsync(e =>
                    e.UsuarioId == usuarioId &&
                    e.Ativo &&
                    e.IsPrincipal &&
                    e.Id != enderecoIgnoradoId);

            if (jaExistePrincipal)
                return;

            var novoPrincipal = await _context.TBL_ENDERECO
                .Where(e => e.UsuarioId == usuarioId && e.Ativo && e.Id != enderecoIgnoradoId)
                .OrderBy(e => e.Id)
                .FirstOrDefaultAsync();

            if (novoPrincipal != null)
                novoPrincipal.IsPrincipal = true;
        }

        private static UsuarioEnderecoLeituraDto MapearEndereco(Endereco endereco)
        {
            return new UsuarioEnderecoLeituraDto
            {
                Id = endereco.Id,
                TipoLogradouro = EnumExtensions.GetDisplayName(endereco.TipoLogradouro),
                NomeEndereco = endereco.NomeEndereco,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Cep = endereco.Cep,
                Cidade = endereco.Cidade,
                Uf = endereco.Uf,
                IsPrincipal = endereco.IsPrincipal,
                Ativo = endereco.Ativo
            };
        }
    }
}
