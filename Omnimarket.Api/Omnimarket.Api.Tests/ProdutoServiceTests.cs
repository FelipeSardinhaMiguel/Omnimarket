using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class ProdutoServiceTests
{
    [Fact]
    public async Task CreateAsync_DeveExigirLojaAntesDeCadastrarProduto()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("vendedor-sem-loja");

        var excecao = await Assert.ThrowsAsync<Exception>(() => fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Mouse Gamer",
                Categoria = "Perifericos",
                Sku = "MOUSE-001",
                Preco = 129.90m,
                Estoque = 5,
                Descricao = "Produto de teste"
            },
            usuario.Id));

        Assert.Equal("Crie uma loja antes de cadastrar produtos.", excecao.Message);
    }

    [Fact]
    public async Task CreateAsync_DeveVincularProdutoALojaERetornarDadosDaLoja()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("vendedor-com-loja");
        var loja = await fixture.CriarLojaAsync(usuario.Id, nomeFantasia: "Loja da Ana");

        var produto = await fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Teclado Mecanico",
                Categoria = "Perifericos",
                Sku = "TECLADO-001",
                Preco = 249.90m,
                Estoque = 8,
                Descricao = "Produto de teste"
            },
            usuario.Id);

        fixture.Context.ChangeTracker.Clear();

        var produtoSalvo = await fixture.Context.TBL_PRODUTO
            .Include(p => p.Loja)
            .SingleAsync(p => p.Id == produto.Id);

        Assert.Equal(loja.Id, produtoSalvo.LojaId);
        Assert.Equal("Perifericos", produtoSalvo.Categoria);
        Assert.Equal(loja.NomeFantasia, produto.NomeLoja);
        Assert.Equal(loja.Slug, produto.SlugLoja);
        Assert.Equal(loja.Id, produto.LojaId);
    }

    [Fact]
    public async Task CreateAsync_DeveExigirCategoriaParaProduto()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("seller-sem-categoria");
        await fixture.CriarLojaAsync(usuario.Id);

        var excecao = await Assert.ThrowsAsync<Exception>(() => fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Produto Sem Categoria",
                Sku = "SEM-CAT-001",
                Preco = 99.90m,
                Estoque = 2
            },
            usuario.Id));

        Assert.Equal("Informe uma categoria valida para o produto.", excecao.Message);
    }

    [Fact]
    public async Task CreateAsync_DevePersistirCategoriaInformadaNoProduto()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("seller-categoria");
        await fixture.CriarLojaAsync(usuario.Id);

        var produto = await fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Camiseta Basic",
                Categoria = "Camisetas",
                Sku = "CAM-BASIC",
                Preco = 59.90m,
                Estoque = 7,
                Descricao = "Produto simples"
            },
            usuario.Id);

        var produtoSalvo = await fixture.Context.TBL_PRODUTO.SingleAsync(p => p.Id == produto.Id);

        Assert.Equal("Camisetas", produtoSalvo.Categoria);
        Assert.Equal("Camisetas", produto.Categoria);
    }

    [Fact]
    public async Task GetPagedAsync_DeveFiltrarPorNomeCategoriaEPreco()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("seller-discovery");
        await fixture.CriarLojaAsync(usuario.Id, nomeFantasia: "Tech Center");

        var notebookPro = await fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Notebook Pro",
                Categoria = "Notebooks",
                Sku = "NOTE-PRO",
                Preco = 5200m,
                Estoque = 3,
                Descricao = "Modelo premium"
            },
            usuario.Id);

        _ = await fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Notebook Lite",
                Categoria = "Notebooks",
                Sku = "NOTE-LITE",
                Preco = 3100m,
                Estoque = 5,
                Descricao = "Linha de entrada"
            },
            usuario.Id);

        var resultado = await fixture.ProdutoService.GetPagedAsync(new ProdutoFiltroDto
        {
            Nome = "Notebook Pro",
            Categoria = "Notebooks",
            PrecoMinimo = 5000m,
            PrecoMaximo = 5300m
        });

        Assert.Single(resultado.Items);
        Assert.Equal(notebookPro.Id, resultado.Items.Single().Id);
        Assert.Equal(notebookPro.Sku, resultado.Items.Single().Sku);
        Assert.Equal(5200m, resultado.Items.Single().Preco);
    }

    [Fact]
    public async Task CreateAsync_DevePermitirMesmoSkuEmLojasDiferentes()
    {
        using var fixture = new ServiceTestFixture();
        var vendedorA = await fixture.CriarUsuarioAsync("seller-a");
        var vendedorB = await fixture.CriarUsuarioAsync("seller-b");

        await fixture.CriarLojaAsync(vendedorA.Id, nomeFantasia: "Loja A");
        await fixture.CriarLojaAsync(vendedorB.Id, nomeFantasia: "Loja B");

        var produtoA = await fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Camiseta A",
                Categoria = "Roupas",
                Sku = "SKU-IGUAL-001",
                Preco = 49.90m,
                Estoque = 5
            },
            vendedorA.Id);

        var produtoB = await fixture.ProdutoService.CreateAsync(
            new ProdutoCriacaoDto
            {
                Nome = "Camiseta B",
                Categoria = "Roupas",
                Sku = "SKU-IGUAL-001",
                Preco = 59.90m,
                Estoque = 3
            },
            vendedorB.Id);

        Assert.NotEqual(produtoA.LojaId, produtoB.LojaId);
        Assert.Equal("SKU-IGUAL-001", produtoA.Sku);
        Assert.Equal("SKU-IGUAL-001", produtoB.Sku);
    }
}
