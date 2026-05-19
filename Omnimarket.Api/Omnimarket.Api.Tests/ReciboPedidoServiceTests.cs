using System.Text;
using System.Reflection;
using Omnimarket.Api.Models.Dtos.Pedidos.Recibos;
using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class ReciboPedidoServiceTests
{
    [Fact]
    public async Task GerarReciboPedidoAsync_DeveRetornarPdfParaPedidoPago()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync(quantidade: 2, preco: 35m, estoque: 10);

        var pdf = await fixture.ReciboPedidoService.GerarReciboPedidoAsync(
            scenario.PedidoId,
            scenario.CompradorId);

        Assert.NotNull(pdf);
        Assert.NotEmpty(pdf!);
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(pdf, 0, 5));
    }

    [Fact]
    public async Task GerarReciboPedidoAsync_DeveBloquearPedidoPendente()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPendenteAsync(quantidade: 1, preco: 42m, estoque: 5);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.ReciboPedidoService.GerarReciboPedidoAsync(
                scenario.PedidoId,
                scenario.CompradorId));

        Assert.Equal("O recibo so pode ser gerado para pedidos pagos, enviados ou entregues.", excecao.Message);
    }

    [Fact]
    public async Task GerarReciboPedidoAsync_DeveRetornarNullQuandoPedidoNaoPertenceAoComprador()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync(quantidade: 1, preco: 55m, estoque: 8);

        var pdf = await fixture.ReciboPedidoService.GerarReciboPedidoAsync(
            scenario.PedidoId,
            scenario.VendedorId);

        Assert.Null(pdf);
    }

    [Fact]
    public async Task GerarReciboPedidoParaVendedorAsync_DeveRetornarPdfQuandoPedidoPertenceALoja()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync(quantidade: 1, preco: 60m, estoque: 8);

        var pdf = await fixture.ReciboPedidoService.GerarReciboPedidoParaVendedorAsync(
            scenario.PedidoId,
            scenario.VendedorId);

        Assert.NotNull(pdf);
        Assert.NotEmpty(pdf!);
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(pdf, 0, 5));
    }

    [Fact]
    public async Task GerarReciboPedidoParaVendedorAsync_DeveRetornarNullQuandoPedidoNaoPertenceALoja()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync(quantidade: 1, preco: 45m, estoque: 6);
        var outroVendedor = await fixture.CriarUsuarioAsync("outro-vendedor");
        await fixture.CriarLojaAsync(outroVendedor.Id, nomeFantasia: "Loja Paralela");

        var pdf = await fixture.ReciboPedidoService.GerarReciboPedidoParaVendedorAsync(
            scenario.PedidoId,
            outroVendedor.Id);

        Assert.Null(pdf);
    }

    [Fact]
    public async Task MapearPedidoParaRecibo_DevePriorizarSnapshotsMesmoQuandoProdutoELojaMudarem()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync(quantidade: 1, preco: 55m, estoque: 8);

        var item = await fixture.Context.TBL_ITENS_PEDIDO
            .Include(i => i.Produto)
            .ThenInclude(p => p.Loja)
            .SingleAsync(i => i.PedidoId == scenario.PedidoId);

        var nomeProdutoOriginal = item.NomeProdutoSnapshot;
        var nomeLojaOriginal = item.NomeLojaSnapshot;
        var documentoOriginal = item.DocumentoLojaSnapshot;
        var tipoDocumentoOriginal = item.TipoDocumentoLojaSnapshot;

        item.Produto.Nome = "Produto Alterado Depois";
        item.Produto.Loja.NomeFantasia = "Loja Alterada Depois";
        item.Produto.Loja.DocumentoFiscal = "11144477735";

        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var pedido = await CarregarPedidoAsync(fixture, scenario.PedidoId);
        var recibo = await MapearPedidoParaReciboAsync(fixture, pedido);
        var itemRecibo = Assert.Single(recibo.Itens);

        Assert.Equal("Comprador", recibo.TipoRecibo);
        Assert.Equal(nomeProdutoOriginal, itemRecibo.NomeProduto);
        Assert.Equal(nomeLojaOriginal, itemRecibo.NomeLoja);
        Assert.Equal(
            DocumentoFiscalSnapshotFormatter.Formatar(tipoDocumentoOriginal, documentoOriginal),
            itemRecibo.DocumentoLoja);
    }

    [Fact]
    public async Task MapearPedidoParaRecibo_DeveCalcularTotaisDaLojaQuandoFiltradoParaVendedor()
    {
        using var fixture = new ServiceTestFixture();
        var vendedor1 = await fixture.CriarUsuarioAsync("vendedor-a");
        var vendedor2 = await fixture.CriarUsuarioAsync("vendedor-b");
        var comprador = await fixture.CriarUsuarioAsync("comprador");
        var endereco = await fixture.CriarEnderecoAsync(comprador.Id);

        var loja1 = await fixture.CriarLojaAsync(vendedor1.Id, nomeFantasia: "Loja A", criarOpcaoRetiradaPadrao: false);
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

        var inicioPagamento = await fixture.FinanceiroService.IniciarPagamentoAsync(
            comprador.Id,
            new IniciarPagamentoDto
            {
                PedidoId = pedido.Id,
                FormaPagamentoId = 1
            });

        await fixture.FinanceiroService.ConfirmarPagamentoFakeAsync(
            comprador.Id,
            inicioPagamento.PlanoPagamentoId);

        fixture.Context.ChangeTracker.Clear();

        var pedidoCarregado = await CarregarPedidoAsync(fixture, pedido.Id);
        var recibo = await MapearPedidoParaReciboAsync(fixture, pedidoCarregado, loja1.Id);

        Assert.Equal("Vendedor", recibo.TipoRecibo);
        Assert.Equal(50m, recibo.ValorProdutos);
        Assert.Equal(0m, recibo.ValorFrete);
        Assert.Equal(50m, recibo.ValorTotal);
        Assert.Single(recibo.Itens);
        Assert.Equal(produto1.Nome, recibo.Itens[0].NomeProduto);
    }

    private static async Task<Pedido> CarregarPedidoAsync(ServiceTestFixture fixture, int pedidoId)
    {
        return await fixture.Context.TBL_PEDIDO
            .AsNoTracking()
            .Include(p => p.Usuario)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                    .ThenInclude(p => p.Loja)
            .SingleAsync(p => p.Id == pedidoId);
    }

    private static async Task<ReciboPedidoDto> MapearPedidoParaReciboAsync(
        ServiceTestFixture fixture,
        Pedido pedido,
        int? lojaIdFiltro = null)
    {
        var metodo = typeof(ReciboPedidoService).GetMethod(
            "MapearPedidoParaRecibo",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(metodo);

        var tarefa = metodo!.Invoke(
            fixture.ReciboPedidoService,
            new object?[] { pedido, lojaIdFiltro }) as Task<ReciboPedidoDto>;

        Assert.NotNull(tarefa);
        return await tarefa!;
    }
}
