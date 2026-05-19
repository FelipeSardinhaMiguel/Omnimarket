using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Usuarios;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class UsuarioPerfilService
    {
        private readonly DataContext _context;

        public UsuarioPerfilService(DataContext context)
        {
            _context = context;
        }

        public async Task<UsuarioPerfilLeituraDto?> ObterPerfilAsync(int usuarioId)
        {
            var usuario = await _context.TBL_USUARIO
                .Include(u => u.Telefones)
                .Include(u => u.Enderecos)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                return null;

            return new UsuarioPerfilLeituraDto
            {
                Id = usuario.Id,
                Cpf = usuario.Cpf,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email,
                Role = usuario.Role,
                Telefones = usuario.Telefones
                    .Select(t => new UsuarioPerfilTelefoneLeituraDto
                    {
                        Id = t.Id,
                        NumeroE164 = t.NumeroE164,
                        IsPrincipal = t.IsPrincipal
                    })
                    .ToList(),
                Enderecos = usuario.Enderecos
                    .Where(e => e.Ativo)
                    .OrderByDescending(e => e.IsPrincipal)
                    .ThenBy(e => e.Id)
                    .Select(e => new UsuarioPerfilEnderecoLeituraDto
                    {
                        Id = e.Id,
                        TipoLogradouro = e.TipoLogradouro,
                        NomeEndereco = e.NomeEndereco,
                        Numero = e.Numero,
                        Cep = e.Cep,
                        Cidade = e.Cidade,
                        Uf = e.Uf,
                        IsPrincipal = e.IsPrincipal,
                        Ativo = e.Ativo
                    })
                    .ToList()
            };
        }

        public async Task<Usuario?> AtualizarAsync(int usuarioId, UsuarioAtualizarDto dto)
        {
            var usuario = await _context.TBL_USUARIO.FindAsync(usuarioId);

            if (usuario == null)
                return null;

            var emailNormalizado = dto.Email.ToLower().Trim();

            if (usuario.Email != emailNormalizado)
            {
                var emailExiste = await _context.TBL_USUARIO
                    .AnyAsync(x => x.Email == emailNormalizado);

                if (emailExiste)
                    throw new InvalidOperationException("Email ja esta em uso.");
            }

            usuario.Nome = dto.Nome.Trim();
            usuario.Sobrenome = dto.Sobrenome.Trim();
            usuario.Email = emailNormalizado;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                Criptografia.CriarPasswordHash(dto.Password, out byte[] hash, out byte[] salt);
                usuario.PasswordHash = hash;
                usuario.PasswordSalt = salt;
            }

            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}
