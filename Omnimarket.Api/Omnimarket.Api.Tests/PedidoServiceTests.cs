using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class PedidoServiceTests
{
    [Fact]
    public async Task CriarPedido_DeveCriarPedidoPendenteEReduzirEstoque()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPendenteAsync(quantidade: 2, preco: 35m, estoque: 10);

        fixture.Context.ChangeTracker.Clear();

        var pedido = await fixture.Context.TBL_PEDIDO
            .Include(p => p.Itens)
            .SingleAsync(p => p.Id == scenario.PedidoId);

        var produto = await fixture.Context.TBL_PRODUTO
            .Include(p => p.Loja)
            .SingleAsync(p => p.Id == scenario.ProdutoId);

        Assert.Equal(StatusPedido.Pendente, pedido.StatusPedidosId);
        Assert.Equal(70m, pedido.ValorTotalProdutos);
        Assert.Equal(0m, pedido.ValorFrete);
        Assert.Equal(70m, pedido.ValorTotalPedido);
        Assert.Single(pedido.Itens);
        Assert.Equal(2, pedido.Itens[0].Quantidade);
        Assert.Equal(8, produto.Estoque);
        Assert.Equal(produto.Nome, pedido.Itens[0].NomeProdutoSnapshot);
        Assert.Equal(produto.Loja.NomeFantasia, pedido.Itens[0].NomeLojaSnapshot);
        Assert.Equal(produto.Loja.DocumentoFiscal, pedido.Itens[0].DocumentoLojaSnapshot);
        Assert.Equal(produto.Loja.TipoDocumentoFiscal.ToString(), pedido.Itens[0].TipoDocumentoLojaSnapshot);
    }

    [Fact]
    public async Task CriarPedido_DeveUsarCarrinhoQuandoBodyVierSemItens()
    {
        using var fixture = new ServiceTestFixture();
        var vendedor = await fixture.CriarUsuarioAsync("vendedor");
        var comprador = await fixture.CriarUsuarioAsync("comprador");
        var endereco = await fixture.CriarEnderecoAsync(comprador.Id);
        var produto = await fixture.CriarProdutoAsync(vendedor.Id, preco: 25m, estoque: 10);

        await fixture.CarrinhoService.AdicionarItemAsync(
            comprador.Id,
            new CarrinhoAdicionarDto
            {
                ProdutoId = produto.Id,
                Quantidade = 3
            });

        var pedido = await fixture.PedidoService.CriarPedido(
            comprador.Id,
            new PedidoDto
            {
                EnderecoId = endereco.Id,
                TipoEntregaId = (int)TipoEntrega.EntregaLocal,
                Observacao = "Pedido gerado a partir do carrinho"
            });

        fixture.Context.ChangeTracker.Clear();

        var pedidoSalvo = await fixture.Context.TBL_PEDIDO
            .Include(p => p.Itens)
            .SingleAsync(p => p.Id == pedido.Id);

        var carrinho = await fixture.Context.TBL_CARRINHO
            .Include(c => c.Itens)
            .SingleAsync(c => c.UsuarioId == comprador.Id);

        Assert.Equal(StatusPedido.Pendente, pedidoSalvo.StatusPedidosId);
        Assert.Equal(75m, pedidoSalvo.ValorTotalPedido);
        Assert.Single(pedidoSalvo.Itens);
        Assert.Equal(3, pedidoSalvo.Itens[0].Quantidade);
        Assert.Empty(carrinho.Itens);
    }

    [Fact]
    public async Task CriarPedido_DeveBloquearCompraDoProprioProdutoMesmoComLojaVinculada()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("dono-da-loja");
        await fixture.CriarLojaAsync(usuario.Id, nomeFantasia: "Loja do Carlos");
        var endereco = await fixture.CriarEnderecoAsync(usuario.Id);
        var produto = await fixture.CriarProdutoAsync(usuario.Id, preco: 90m, estoque: 4);

        var excecao = await Assert.ThrowsAsync<Exception>(() => fixture.PedidoService.CriarPedido(
            usuario.Id,
            new PedidoDto
            {
                EnderecoId = endereco.Id,
                TipoEntregaId = (int)TipoEntrega.Retirada,
                Itens =
                [
                    new ItemPedidoDto
                    {
                        ProdutoId = produto.Id,
                        Quantidade = 1
                    }
                ]
            }));

        Assert.Equal($"Voce nao pode comprar o proprio produto {produto.Id}.", excecao.Message);
    }

    [Fact]
    public async Task CancelarPedido_DeveRestaurarEstoqueEEstornarFluxoFinanceiro()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync(quantidade: 2, preco: 40m, estoque: 10);

        var cancelado = await fixture.PedidoService.CancelarPedido(scenario.PedidoId, scenario.CompradorId);

        fixture.Context.ChangeTracker.Clear();

        var pedido = await fixture.Context.TBL_PEDIDO
            .SingleAsync(p => p.Id == scenario.PedidoId);

        var produto = await fixture.Context.TBL_PRODUTO
            .SingleAsync(p => p.Id == scenario.ProdutoId);

        var plano = await fixture.Context.TBL_PLANO_PAGAMENTO
            .SingleAsync(p => p.Id == scenario.PlanoPagamentoId);

        var venda = await fixture.Context.TBL_VENDA
            .SingleAsync(v => v.PedidoId == scenario.PedidoId);

        Assert.True(cancelado);
        Assert.Equal(StatusPedido.Cancelado, pedido.StatusPedidosId);
        Assert.Equal(10, produto.Estoque);
        Assert.Equal(StatusPagamento.Estornado, plano.StatusPagamento);
        Assert.Equal(StatusVenda.Cancelada, venda.StatusVenda);
    }

    [Fact]
    public async Task MarcarPedidoComoEnviadoEConfirmarEntrega_DevemAtualizarPedidoEVenda()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync();

        var pedidoEnviado = await fixture.PedidoService.MarcarPedidoComoEnviadoAsync(scenario.PedidoId);
        Assert.NotNull(pedidoEnviado);
        Assert.Equal(StatusPedido.Enviado, pedidoEnviado!.StatusPedidosId);

        fixture.Context.ChangeTracker.Clear();

        var vendaEnviada = await fixture.Context.TBL_VENDA
            .SingleAsync(v => v.PedidoId == scenario.PedidoId);

        Assert.Equal(StatusVenda.Enviada, vendaEnviada.StatusVenda);

        var pedidoEntregue = await fixture.PedidoService.ConfirmarEntregaPedidoAsync(
            scenario.PedidoId,
            scenario.CompradorId);

        Assert.NotNull(pedidoEntregue);
        Assert.Equal(StatusPedido.Entregue, pedidoEntregue!.StatusPedidosId);

        fixture.Context.ChangeTracker.Clear();

        var pedidoAtualizado = await fixture.Context.TBL_PEDIDO
            .SingleAsync(p => p.Id == scenario.PedidoId);

        var vendaConcluida = await fixture.Context.TBL_VENDA
            .SingleAsync(v => v.PedidoId == scenario.PedidoId);

        Assert.Equal(StatusPedido.Entregue, pedidoAtualizado.StatusPedidosId);
        Assert.Equal(StatusVenda.Concluida, vendaConcluida.StatusVenda);
    }

    [Fact]
    public async Task CancelarPedido_DeveFalharQuandoPedidoJaFoiEnviado()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync();

        await fixture.PedidoService.MarcarPedidoComoEnviadoAsync(scenario.PedidoId);

        var excecao = await Assert.ThrowsAsync<Exception>(() =>
            fixture.PedidoService.CancelarPedido(scenario.PedidoId, scenario.CompradorId));

        Assert.Equal(
            "Pedido enviado nao pode ser cancelado pelo cliente. Nesse caso o ideal e abrir um atendimento para devolucao.",
            excecao.Message);
    }

    [Fact]
    public async Task BuscarPedido_DeveRetornarDtoComEnderecoEItens()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPendenteAsync(quantidade: 2, preco: 25m, estoque: 10);

        var pedido = await fixture.PedidoService.BuscarPedido(scenario.PedidoId, scenario.CompradorId);

        Assert.NotNull(pedido);
        Assert.Equal(scenario.PedidoId, pedido!.Id);
        Assert.Equal("Entrega local", pedido.TipoEntrega);
        Assert.Equal("Sao Paulo", pedido.CidadeEntrega);
        Assert.Equal("SP", pedido.UfEntrega);
        Assert.Single(pedido.Itens);
        Assert.Equal(scenario.ProdutoId, pedido.Itens[0].ProdutoId);
        Assert.Equal(2, pedido.Itens[0].Quantidade);
        Assert.False(string.IsNullOrWhiteSpace(pedido.Itens[0].NomeProduto));
        Assert.False(string.IsNullOrWhiteSpace(pedido.Itens[0].NomeLoja));
    }
}
