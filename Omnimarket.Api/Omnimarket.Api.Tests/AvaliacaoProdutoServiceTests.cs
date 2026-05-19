using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class AvaliacaoProdutoServiceTests
{
    [Fact]
    public async Task CriarAsync_DeveExigirPedidoEntregueParaAvaliar()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoPagoAsync();

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.AvaliacaoProdutoService.CriarAsync(
                scenario.ProdutoId,
                scenario.CompradorId,
                new ProdutoAvaliacaoCriacaoDto
                {
                    PedidoId = scenario.PedidoId,
                    NotaProduto = 5,
                    NotaLoja = 4,
                    Comentario = "Ainda nao recebi."
                }));

        Assert.Equal("Somente pedidos entregues podem ser avaliados.", excecao.Message);
    }

    [Fact]
    public async Task CriarAsync_DeveSalvarAvaliacaoEAtualizarReputacaoDoProdutoEDaLoja()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoEntregueAsync();

        var avaliacao = await fixture.AvaliacaoProdutoService.CriarAsync(
            scenario.ProdutoId,
            scenario.CompradorId,
            new ProdutoAvaliacaoCriacaoDto
            {
                PedidoId = scenario.PedidoId,
                NotaProduto = 5,
                NotaLoja = 4,
                Titulo = "Compra muito boa",
                Comentario = "Produto chegou certo e a loja cumpriu o prazo.",
                RecomendaProduto = true
            });

        fixture.Context.ChangeTracker.Clear();

        var produto = await fixture.ProdutoService.GetByIdAsync(scenario.ProdutoId);
        var loja = await fixture.Context.TBL_LOJA.SingleAsync(l => l.UsuarioId == scenario.VendedorId);

        Assert.NotNull(produto);
        Assert.Equal(5, avaliacao.NotaProduto);
        Assert.Equal(4, avaliacao.NotaLoja);
        Assert.True(avaliacao.CompraVerificada);
        Assert.Equal(5d, produto!.MediaAvaliacao);
        Assert.Equal(1, produto.TotalAvaliacoes);
        Assert.Equal(4d, loja.MediaAvaliacao);
        Assert.Equal(1, loja.TotalAvaliacoes);
    }

    [Fact]
    public async Task CriarAsync_DeveBloquearAvaliacaoDuplicadaDoMesmoPedidoEProduto()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoEntregueAsync();

        _ = await fixture.AvaliacaoProdutoService.CriarAsync(
            scenario.ProdutoId,
            scenario.CompradorId,
            new ProdutoAvaliacaoCriacaoDto
            {
                PedidoId = scenario.PedidoId,
                NotaProduto = 5,
                NotaLoja = 5
            });

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.AvaliacaoProdutoService.CriarAsync(
                scenario.ProdutoId,
                scenario.CompradorId,
                new ProdutoAvaliacaoCriacaoDto
                {
                    PedidoId = scenario.PedidoId,
                    NotaProduto = 4,
                    NotaLoja = 4
                }));

        Assert.Equal("Voce ja avaliou este produto nesse pedido.", excecao.Message);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRecalcularAgregadosAposEditarAvaliacao()
    {
        using var fixture = new ServiceTestFixture();
        var scenario = await fixture.CriarPedidoEntregueAsync();

        var avaliacao = await fixture.AvaliacaoProdutoService.CriarAsync(
            scenario.ProdutoId,
            scenario.CompradorId,
            new ProdutoAvaliacaoCriacaoDto
            {
                PedidoId = scenario.PedidoId,
                NotaProduto = 5,
                NotaLoja = 5,
                Comentario = "Excelente."
            });

        var avaliacaoAtualizada = await fixture.AvaliacaoProdutoService.AtualizarAsync(
            scenario.ProdutoId,
            avaliacao.Id,
            scenario.CompradorId,
            new ProdutoAvaliacaoAtualizacaoDto
            {
                NotaProduto = 3,
                NotaLoja = 2,
                Comentario = "Revisei minha percepcao apos o uso."
            });

        fixture.Context.ChangeTracker.Clear();

        var produto = await fixture.ProdutoService.GetByIdAsync(scenario.ProdutoId);
        var loja = await fixture.Context.TBL_LOJA.SingleAsync(l => l.UsuarioId == scenario.VendedorId);

        Assert.NotNull(avaliacaoAtualizada);
        Assert.NotNull(produto);
        Assert.Equal(3, avaliacaoAtualizada!.NotaProduto);
        Assert.Equal(2, avaliacaoAtualizada.NotaLoja);
        Assert.Equal(3d, produto!.MediaAvaliacao);
        Assert.Equal(2d, loja.MediaAvaliacao);
    }
}
