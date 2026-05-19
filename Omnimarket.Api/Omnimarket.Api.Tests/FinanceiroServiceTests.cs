using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class FinanceiroServiceTests
{
    [Fact]
    public async Task IniciarPagamentoAsync_DeveCriarPlanoEVendaSimples()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPendenteAsync(quantidade: 2, preco: 30m, estoque: 10);

        var resposta = await fixture.FinanceiroService.IniciarPagamentoAsync(
            scenario.CompradorId,
            new IniciarPagamentoDto
            {
                PedidoId = scenario.PedidoId,
                FormaPagamentoId = 1
            });

        fixture.Context.ChangeTracker.Clear();

        var plano = await fixture.Context.TBL_PLANO_PAGAMENTO
            .SingleAsync(p => p.Id == resposta.PlanoPagamentoId);

        var venda = await fixture.Context.TBL_VENDA
            .SingleAsync(v => v.PedidoId == scenario.PedidoId);

        Assert.Equal(StatusPagamento.Pendente, plano.StatusPagamento);
        Assert.Equal(1, plano.FormaPagamentoId);
        Assert.Equal(1, resposta.FormaPagamentoId);
        Assert.Equal(StatusVenda.Criada, venda.StatusVenda);
        Assert.Equal(venda.ValorBruto, venda.ValorLiquido);
    }

    [Fact]
    public async Task ConfirmarPagamentoFakeAsync_DeveAprovarPagamentoEMarcarPedidoComoPago()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPendenteAsync(quantidade: 1, preco: 80m, estoque: 5);

        var inicioPagamento = await fixture.FinanceiroService.IniciarPagamentoAsync(
            scenario.CompradorId,
            new IniciarPagamentoDto
            {
                PedidoId = scenario.PedidoId,
                FormaPagamentoId = 1
            });

        var confirmacao = await fixture.FinanceiroService.ConfirmarPagamentoFakeAsync(
            scenario.CompradorId,
            inicioPagamento.PlanoPagamentoId);

        fixture.Context.ChangeTracker.Clear();

        var pedido = await fixture.Context.TBL_PEDIDO
            .SingleAsync(p => p.Id == scenario.PedidoId);

        var plano = await fixture.Context.TBL_PLANO_PAGAMENTO
            .SingleAsync(p => p.Id == inicioPagamento.PlanoPagamentoId);

        var venda = await fixture.Context.TBL_VENDA
            .SingleAsync(v => v.PedidoId == scenario.PedidoId);

        Assert.Equal(StatusPagamento.Aprovado, confirmacao.StatusPagamento);
        Assert.Equal(1, confirmacao.FormaPagamentoId);
        Assert.Equal(StatusPedido.Pago, pedido.StatusPedidosId);
        Assert.Equal(StatusPagamento.Aprovado, plano.StatusPagamento);
        Assert.False(string.IsNullOrWhiteSpace(confirmacao.GatewayTransactionId));
        Assert.Equal(StatusVenda.Paga, venda.StatusVenda);
    }

    [Fact]
    public async Task IniciarPagamentoAsync_DeveCriarVendasMultilojaSemFrete()
    {
        using var fixture = new ServiceTestFixture();
        var vendedor1 = await fixture.CriarUsuarioAsync("vendedor-a");
        var vendedor2 = await fixture.CriarUsuarioAsync("vendedor-b");
        var comprador = await fixture.CriarUsuarioAsync("comprador");
        var endereco = await fixture.CriarEnderecoAsync(comprador.Id);

        await fixture.CriarLojaAsync(vendedor1.Id, nomeFantasia: "Loja A", criarOpcaoRetiradaPadrao: false);
        await fixture.CriarLojaAsync(vendedor2.Id, nomeFantasia: "Loja B", criarOpcaoRetiradaPadrao: false);

        var produto1 = await fixture.CriarProdutoAsync(vendedor1.Id, preco: 50m, estoque: 5);
        var produto2 = await fixture.CriarProdutoAsync(vendedor2.Id, preco: 70m, estoque: 5);

        var pedido = await fixture.PedidoService.CriarPedido(
            comprador.Id,
            new PedidoDto
            {
                EnderecoId = endereco.Id,
                TipoEntregaId = (int)TipoEntrega.Correios,
                Itens =
                [
                    new ItemPedidoDto
                    {
                        ProdutoId = produto1.Id,
                        Quantidade = 1
                    },
                    new ItemPedidoDto
                    {
                        ProdutoId = produto2.Id,
                        Quantidade = 1
                    }
                ]
            });

        await fixture.FinanceiroService.IniciarPagamentoAsync(
            comprador.Id,
            new IniciarPagamentoDto
            {
                PedidoId = pedido.Id,
                FormaPagamentoId = 1
            });

        fixture.Context.ChangeTracker.Clear();

        var pedidoSalvo = await fixture.Context.TBL_PEDIDO
            .SingleAsync(p => p.Id == pedido.Id);

        var vendas = await fixture.Context.TBL_VENDA
            .Where(v => v.PedidoId == pedido.Id)
            .OrderBy(v => v.VendedorId)
            .ToListAsync();

        Assert.Equal(0m, pedidoSalvo.ValorFrete);
        Assert.Equal(2, vendas.Count);
        Assert.Contains(vendas, v => v.ValorBruto == 50m && v.ValorLiquido == 50m);
        Assert.Contains(vendas, v => v.ValorBruto == 70m && v.ValorLiquido == 70m);
    }
}
