using Omnimarket.Api.Models.Dtos.Pedidos.Carrinho;
using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class CarrinhoServiceTests
{
    [Fact]
    public async Task AdicionarItemAsync_DeveCriarCarrinhoERetornarTotais()
    {
        using var fixture = new ServiceTestFixture();
        var vendedor = await fixture.CriarUsuarioAsync("vendedor-carrinho");
        var comprador = await fixture.CriarUsuarioAsync("comprador-carrinho");

        await fixture.CriarLojaAsync(vendedor.Id, nomeFantasia: "Loja do Carrinho");
        var produto = await fixture.CriarProdutoAsync(vendedor.Id, preco: 19.90m, estoque: 8);

        var carrinho = await fixture.CarrinhoService.AdicionarItemAsync(
            comprador.Id,
            new CarrinhoAdicionarDto
            {
                ProdutoId = produto.Id,
                Quantidade = 2
            });

        Assert.NotNull(carrinho.CarrinhoId);
        Assert.Single(carrinho.Itens);
        Assert.Equal(2, carrinho.TotalItens);
        Assert.Equal(39.80m, carrinho.ValorTotal);
        Assert.Equal(produto.Id, carrinho.Itens[0].ProdutoId);
        Assert.Equal("Loja do Carrinho", carrinho.Itens[0].NomeLoja);
    }
}
