using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models.Dtos.Financeiro;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Services.Interfaces;

namespace Omnimarket.Api.Services
{
    public class FinanceiroService
    {
        private readonly DataContext _context;
        private readonly IGatewayPagamentoService _gatewayPagamentoService;

        public FinanceiroService(
            DataContext context,
            IGatewayPagamentoService gatewayPagamentoService)
        {
            _context = context;
            _gatewayPagamentoService = gatewayPagamentoService;
        }

        public async Task<IniciarPagamentoRespostaDto> IniciarPagamentoAsync(int compradorId, IniciarPagamentoDto dto)
        {
            var pedido = await _context.TBL_PEDIDO
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .ThenInclude(produto => produto.Loja)
                .FirstOrDefaultAsync(p => p.Id == dto.PedidoId);

            if (pedido == null)
                throw new Exception("Pedido nao encontrado.");

            if (pedido.UsuarioId != compradorId)
                throw new Exception("Voce nao pode iniciar pagamento para pedido de outro usuario.");

            if (pedido.StatusPedidosId == StatusPedido.Cancelado)
                throw new Exception("Pedido cancelado nao pode ser pago.");

            if (pedido.StatusPedidosId == StatusPedido.Pago ||
                pedido.StatusPedidosId == StatusPedido.Enviado ||
                pedido.StatusPedidosId == StatusPedido.Entregue)
            {
                throw new Exception("Este pedido ja foi processado para pagamento.");
            }

            if (pedido.Itens.Count == 0)
                throw new Exception("Pedido sem itens nao pode ser pago.");

            if (pedido.Itens.Any(i => i.Produto.Loja == null || !i.Produto.Loja.Ativa))
                throw new Exception("Existe produto vinculado a uma loja inativa neste pedido.");

            if (pedido.Itens.Any(i => i.Produto.Loja.UsuarioId == compradorId))
                throw new Exception("Voce nao pode comprar o proprio produto.");

            var planoExistente = await _context.TBL_PLANO_PAGAMENTO
                .FirstOrDefaultAsync(p =>
                    p.PedidoId == pedido.Id &&
                    p.StatusPagamento != StatusPagamento.Cancelado &&
                    p.StatusPagamento != StatusPagamento.Recusado &&
                    p.StatusPagamento != StatusPagamento.Estornado);

            if (planoExistente != null)
                throw new Exception("Este pedido ja possui um plano de pagamento ativo.");

            var vendasExistentes = await _context.TBL_VENDA.AnyAsync(v => v.PedidoId == pedido.Id);
            if (vendasExistentes)
                throw new Exception("Este pedido ja possui vendas registradas.");

            await ValidarFormaPagamentoAsync(dto.FormaPagamentoId);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var plano = new PlanoPagamento
            {
                PedidoId = pedido.Id,
                FormaPagamentoId = dto.FormaPagamentoId,
                ValorTotal = pedido.ValorTotalPedido,
                StatusPagamento = StatusPagamento.Pendente,
                DataCriacao = DateTime.UtcNow
            };

            var vendasCriadas = pedido.Itens
                .GroupBy(i => i.Produto.Loja.UsuarioId)
                .Select(grupo =>
                {
                    var valorBruto = Math.Round(grupo.Sum(i => i.ValorTotal), 2, MidpointRounding.AwayFromZero);

                    return new Venda
                    {
                        PedidoId = pedido.Id,
                        VendedorId = grupo.Key,
                        ValorBruto = valorBruto,
                        ValorLiquido = valorBruto,
                        StatusVenda = StatusVenda.Criada,
                        DataCriacao = DateTime.UtcNow
                    };
                })
                .ToList();

            await _context.TBL_PLANO_PAGAMENTO.AddAsync(plano);
            await _context.TBL_VENDA.AddRangeAsync(vendasCriadas);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new IniciarPagamentoRespostaDto
            {
                PedidoId = pedido.Id,
                PlanoPagamentoId = plano.Id,
                FormaPagamentoId = plano.FormaPagamentoId,
                ValorTotal = plano.ValorTotal,
                StatusPagamento = plano.StatusPagamento,
                Vendas = vendasCriadas
                    .OrderBy(v => v.Id)
                    .Select(v => new VendaFinanceiraResumoDto
                    {
                        VendaId = v.Id,
                        VendedorId = v.VendedorId,
                        ValorBruto = v.ValorBruto,
                        ValorComissao = v.ValorBruto - v.ValorLiquido,
                        ValorLiquido = v.ValorLiquido,
                        StatusVenda = v.StatusVenda
                    })
                    .ToList()
            };
        }

        public async Task<ConfirmarPagamentoRespostaDto> ConfirmarPagamentoFakeAsync(int compradorId, int planoPagamentoId)
        {
            var plano = await _context.TBL_PLANO_PAGAMENTO
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.Id == planoPagamentoId);

            if (plano == null)
                throw new Exception("Plano de pagamento nao encontrado.");

            if (plano.Pedido.UsuarioId != compradorId)
                throw new Exception("Voce nao pode confirmar pagamento de outro usuario.");

            if (plano.StatusPagamento == StatusPagamento.Aprovado)
                throw new Exception("Este pagamento ja foi confirmado.");

            if (plano.StatusPagamento == StatusPagamento.Cancelado ||
                plano.StatusPagamento == StatusPagamento.Estornado)
            {
                throw new Exception("Este pagamento nao pode mais ser confirmado.");
            }

            var vendas = await _context.TBL_VENDA
                .Where(v => v.PedidoId == plano.PedidoId)
                .ToListAsync();

            if (vendas.Count == 0)
                throw new Exception("Nao existem vendas vinculadas ao pedido.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var retornoGateway = await _gatewayPagamentoService.ProcessarPagamentoAsync(plano.ValorTotal, plano.Id);
            var agora = DateTime.UtcNow;

            plano.StatusPagamento = retornoGateway.StatusPagamento;

            if (retornoGateway.StatusPagamento == StatusPagamento.Aprovado)
            {
                plano.Pedido.StatusPedidosId = StatusPedido.Pago;

                foreach (var venda in vendas)
                {
                    venda.StatusVenda = StatusVenda.Paga;
                    venda.DataAtualizacao = agora;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ConfirmarPagamentoRespostaDto
            {
                PlanoPagamentoId = plano.Id,
                PedidoId = plano.PedidoId,
                FormaPagamentoId = plano.FormaPagamentoId,
                StatusPagamento = plano.StatusPagamento,
                GatewayTransactionId = retornoGateway.GatewayTransactionId,
                DataConfirmacao = retornoGateway.StatusPagamento == StatusPagamento.Aprovado ? agora : null,
                QuantidadeVendasAtualizadas = vendas.Count
            };
        }

        public async Task CancelarFluxoFinanceiroDoPedidoAsync(int pedidoId, int usuarioResponsavelId, string origem)
        {
            var plano = await _context.TBL_PLANO_PAGAMENTO
                .Where(p => p.PedidoId == pedidoId)
                .Where(p =>
                    p.StatusPagamento != StatusPagamento.Cancelado &&
                    p.StatusPagamento != StatusPagamento.Estornado &&
                    p.StatusPagamento != StatusPagamento.Recusado)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (plano == null)
                return;

            var statusCancelamento = plano.StatusPagamento == StatusPagamento.Aprovado
                ? StatusPagamento.Estornado
                : StatusPagamento.Cancelado;

            plano.StatusPagamento = statusCancelamento;

            var vendas = await _context.TBL_VENDA
                .Where(v => v.PedidoId == pedidoId)
                .ToListAsync();

            var agora = DateTime.UtcNow;

            foreach (var venda in vendas)
            {
                venda.StatusVenda = StatusVenda.Cancelada;
                venda.DataAtualizacao = agora;
            }
        }

        private async Task ValidarFormaPagamentoAsync(int formaPagamentoId)
        {
            var formaPagamentoAtiva = await _context.TBL_FORMA_PAGAMENTO
                .AnyAsync(f => f.Id == formaPagamentoId && f.Ativo);

            if (!formaPagamentoAtiva)
                throw new Exception("Forma de pagamento invalida ou inativa.");
        }
    }
}
