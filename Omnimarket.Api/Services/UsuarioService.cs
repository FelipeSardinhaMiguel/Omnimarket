using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;

namespace Omnimarket.Api.Services
{
    public class UsuarioService
    {
        private readonly DataContext _context;

        public UsuarioService(DataContext context)
        {
            _context = context;
        }

        // Considera vendedor qualquer usuario que tenha pelo menos um produto cadastrado.
        public async Task<bool> UsuarioVendedor(int usuarioId)
        {
            return await _context.TBL_PRODUTO
                .AnyAsync(p => p.Loja.UsuarioId == usuarioId);
        }

        // Considera comprador qualquer usuario que ja tenha um carrinho vinculado.
        public async Task<bool> UsuarioComprador(int usuarioId)
        {
            return await _context.TBL_CARRINHO
                .AnyAsync(c => c.UsuarioId == usuarioId);
        }
    }
}
