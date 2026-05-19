using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Usuarios.Telefones;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class TelefoneService
    {
        private readonly DataContext _context;

        public TelefoneService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<UsuarioTelefoneLeituraDto>> ListarAsync(int usuarioId)
        {
            var telefones = await _context.TBL_TELEFONE
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            return telefones.Select(MapearTelefone).ToList();
        }

        public async Task<UsuarioTelefoneLeituraDto?> ObterAsync(int usuarioId, int telefoneId)
        {
            var telefone = await _context.TBL_TELEFONE
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.Id == telefoneId);

            return telefone == null ? null : MapearTelefone(telefone);
        }

        public async Task CriarAsync(int usuarioId, UsuarioTelefoneDto dto)
        {
            var resultado = ValidadorTelefone.ValidarCelularBr(dto.Ddd, dto.Numero);
            if (!resultado.Valido)
                throw new InvalidOperationException("Telefone invalido.");

            var telefonesExistentes = await _context.TBL_TELEFONE
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            var seraPrincipal = dto.IsPrincipal == true || telefonesExistentes.Count == 0;

            if (seraPrincipal)
            {
                foreach (var telefoneExistente in telefonesExistentes)
                    telefoneExistente.IsPrincipal = false;
            }

            var telefone = new Telefone
            {
                UsuarioId = usuarioId,
                NumeroE164 = resultado.E164!,
                IsPrincipal = seraPrincipal
            };

            await _context.TBL_TELEFONE.AddAsync(telefone);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AtualizarAsync(int usuarioId, int telefoneId, UsuarioTelefoneDto dto)
        {
            var telefone = await _context.TBL_TELEFONE
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.Id == telefoneId);

            if (telefone == null)
                return false;

            var resultado = ValidadorTelefone.ValidarCelularBr(dto.Ddd, dto.Numero);
            if (!resultado.Valido)
                throw new InvalidOperationException("Telefone invalido.");

            telefone.NumeroE164 = resultado.E164!;

            if (dto.IsPrincipal == true)
            {
                var telefones = await _context.TBL_TELEFONE
                    .Where(t => t.UsuarioId == usuarioId)
                    .ToListAsync();

                foreach (var telefoneExistente in telefones)
                    telefoneExistente.IsPrincipal = false;

                telefone.IsPrincipal = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverAsync(int usuarioId, int telefoneId)
        {
            var telefone = await _context.TBL_TELEFONE
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.Id == telefoneId);

            if (telefone == null)
                return false;

            var telefones = await _context.TBL_TELEFONE
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            if (telefones.Count <= 1)
                throw new InvalidOperationException("Nao pode remover o ultimo telefone.");

            if (telefone.IsPrincipal)
            {
                var novoPrincipal = telefones.FirstOrDefault(t => t.Id != telefoneId);
                if (novoPrincipal != null)
                    novoPrincipal.IsPrincipal = true;
            }

            _context.TBL_TELEFONE.Remove(telefone);
            await _context.SaveChangesAsync();
            return true;
        }

        private static UsuarioTelefoneLeituraDto MapearTelefone(Telefone telefone)
        {
            return new UsuarioTelefoneLeituraDto
            {
                Id = telefone.Id,
                Numero = telefone.NumeroE164,
                IsPrincipal = telefone.IsPrincipal
            };
        }
    }
}
