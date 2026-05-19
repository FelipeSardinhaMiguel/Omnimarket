using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Extensions;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services
{
    public class AdminVendaService
    {
        private readonly DataContext _context;

        public AdminVendaService(DataContext context)
        {
            _context = context;
        }

        public async Task<PageResult<AdminVendaFinanceiraDto>> ListarVendasAsync(
            StatusVenda? statusVenda,
            int page,
            int pageSize)
        {
            var query = _context.TBL_VENDA
                .AsNoTracking()
                .AsQueryable();

            if (statusVenda.HasValue)
                query = query.Where(v => v.StatusVenda == statusVenda.Value);

            return await query
                .OrderByDescending(v => v.DataCriacao)
                .Select(v => new AdminVendaFinanceiraDto
                {
                    Id = v.Id,
                    PedidoId = v.PedidoId,
                    VendedorId = v.VendedorId,
                    NomeVendedor = v.Vendedor.Nome + " " + v.Vendedor.Sobrenome,
                    ValorBruto = v.ValorBruto,
                    ValorComissao = v.ValorBruto - v.ValorLiquido,
                    ValorLiquido = v.ValorLiquido,
                    StatusVenda = v.StatusVenda,
                    DataCriacao = v.DataCriacao,
                    DataAtualizacao = v.DataAtualizacao
                })
                .ToPagedResultAsync(page, pageSize);
        }
    }
}
