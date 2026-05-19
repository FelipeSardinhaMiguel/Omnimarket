using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Extensions;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class AdminUsuarioService
    {
        private readonly DataContext _context;

        public AdminUsuarioService(DataContext context)
        {
            _context = context;
        }

        public async Task<PageResult<AdminUsuarioDto>> ListarUsuariosAsync(
            string? busca,
            string? role,
            int page,
            int pageSize)
        {
            var query = _context.TBL_USUARIO
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = $"%{busca.Trim()}%";
                query = query.Where(u =>
                    EF.Functions.Like(u.Nome, termo) ||
                    EF.Functions.Like(u.Sobrenome, termo) ||
                    EF.Functions.Like(u.Email, termo) ||
                    EF.Functions.Like(u.Cpf, termo));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleNormalizada = RolesSistema.Normalizar(role);
                if (roleNormalizada == null)
                {
                    return await Enumerable.Empty<AdminUsuarioDto>()
                        .AsQueryable()
                        .ToPagedResultAsync(page, pageSize);
                }

                query = query.Where(u => u.Role == roleNormalizada);
            }

            return await query
                .OrderByDescending(u => u.DataCadastro)
                .Select(u => new AdminUsuarioDto
                {
                    Id = u.Id,
                    Nome = u.Nome + " " + u.Sobrenome,
                    Email = u.Email,
                    Cpf = u.Cpf,
                    Role = u.Role,
                    DataCadastro = u.DataCadastro,
                    DataAcesso = u.DataAcesso,
                    PossuiLoja = u.Loja != null,
                    LojaAtiva = u.Loja != null && u.Loja.Ativa,
                    TotalProdutos = u.Loja == null ? 0 : u.Loja.Produtos.Count,
                    TotalPedidos = u.Pedidos.Count
                })
                .ToPagedResultAsync(page, pageSize);
        }

        public async Task<AdminUsuarioDto?> AtualizarRoleUsuarioAsync(
            int usuarioId,
            AdminAtualizarRoleDto dto,
            int adminLogadoId)
        {
            var roleNormalizada = RolesSistema.Normalizar(dto.Role);
            if (roleNormalizada == null)
                throw new InvalidOperationException("Role invalida.");

            if (usuarioId == adminLogadoId && roleNormalizada != RolesSistema.Admin)
                throw new InvalidOperationException("Voce nao pode remover seu proprio acesso admin.");

            var usuario = await _context.TBL_USUARIO
                .Include(u => u.Loja)
                .ThenInclude(l => l!.Produtos)
                .Include(u => u.Pedidos)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                return null;

            if (usuario.Role == RolesSistema.Admin && roleNormalizada != RolesSistema.Admin)
            {
                var totalAdmins = await _context.TBL_USUARIO.CountAsync(u => u.Role == RolesSistema.Admin);
                if (totalAdmins <= 1)
                    throw new InvalidOperationException("Mantenha pelo menos um administrador ativo.");
            }

            usuario.Role = roleNormalizada;
            await _context.SaveChangesAsync();

            return usuario.ToAdminDto();
        }
    }
}
