using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Extensions;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services
{
    public class AdminPedidoService
    {
        private readonly DataContext _context;

        public AdminPedidoService(DataContext context)
        {
            _context = context;
        }

        public async Task<PageResult<AdminPedidoDto>> ListarPedidosAsync(
            string? busca,
            StatusPedido? status,
            int page,
            int pageSize)
        {
            var query = _context.TBL_PEDIDO
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = $"%{busca.Trim()}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Usuario.Nome, termo) ||
                    EF.Functions.Like(p.Usuario.Email, termo) ||
                    EF.Functions.Like(p.CidadeEntrega, termo));
            }

            if (status.HasValue)
                query = query.Where(p => p.StatusPedidosId == status.Value);

            return await query
                .OrderByDescending(p => p.DataPedido)
                .Select(p => new AdminPedidoDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    NomeCliente = p.Usuario.Nome + " " + p.Usuario.Sobrenome,
                    EmailCliente = p.Usuario.Email,
                    Status = p.StatusPedidosId,
                    ValorTotalProdutos = p.ValorTotalProdutos,
                    ValorFrete = p.ValorFrete,
                    ValorTotalPedido = p.ValorTotalPedido,
                    QuantidadeItens = p.Itens.Count,
                    DataPedido = p.DataPedido,
                    CidadeEntrega = p.CidadeEntrega,
                    UfEntrega = p.UfEntrega
                })
                .ToPagedResultAsync(page, pageSize);
        }
    }
}
