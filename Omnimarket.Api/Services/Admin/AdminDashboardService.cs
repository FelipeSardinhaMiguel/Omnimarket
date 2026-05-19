using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class AdminDashboardService
    {
        private readonly DataContext _context;

        public AdminDashboardService(DataContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> ObterDashboardAsync()
        {
            return new AdminDashboardDto
            {
                TotalUsuarios = await _context.TBL_USUARIO.CountAsync(),
                TotalAdmins = await _context.TBL_USUARIO.CountAsync(u => u.Role == RolesSistema.Admin),
                TotalLojas = await _context.TBL_LOJA.CountAsync(),
                TotalLojasAtivas = await _context.TBL_LOJA.CountAsync(l => l.Ativa),
                TotalProdutos = await _context.TBL_PRODUTO.CountAsync(),
                ProdutosPublicados = await _context.TBL_PRODUTO.CountAsync(p => p.StatusPublicacao == StatusProduto.Publicado),
                TotalPedidos = await _context.TBL_PEDIDO.CountAsync(),
                PedidosPendentes = await _context.TBL_PEDIDO.CountAsync(p => p.StatusPedidosId == StatusPedido.Pendente),
                PedidosPagos = await _context.TBL_PEDIDO.CountAsync(p =>
                    p.StatusPedidosId == StatusPedido.Pago ||
                    p.StatusPedidosId == StatusPedido.Enviado ||
                    p.StatusPedidosId == StatusPedido.Entregue),
                ReceitaTotalMarketplace = await _context.TBL_VENDA.SumAsync(v => (decimal?)v.ValorBruto) ?? 0m
            };
        }
    }
}
