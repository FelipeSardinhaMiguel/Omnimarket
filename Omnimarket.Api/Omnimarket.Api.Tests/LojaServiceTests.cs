using Omnimarket.Api.Models.Dtos.Produtos.Lojas;
using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class LojaServiceTests
{
    [Fact]
    public async Task CriarMinhaLojaAsync_DeveClonarEnderecoETelefoneDoUsuario()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("vendedora-pf");
        var enderecoUsuario = await fixture.CriarEnderecoAsync(usuario.Id);
        var telefoneUsuario = await fixture.CriarTelefoneAsync(usuario.Id, numeroE164: "+5511987654321");

        var loja = await fixture.LojaService.CriarMinhaLojaAsync(
            usuario.Id,
            new LojaCriacaoDto
            {
                NomeFantasia = "Atelie da Ana",
                TipoDocumentoFiscal = TipoDocumentoFiscalLoja.CPF,
                DocumentoFiscal = "529.982.247-25",
                EmailContato = "ana@atelie.com",
                UsarEnderecoUsuario = true,
                EnderecoUsuarioId = enderecoUsuario.Id,
                UsarTelefoneUsuario = true,
                TelefoneUsuarioId = telefoneUsuario.Id
            });

        fixture.Context.ChangeTracker.Clear();

        var lojaSalva = await fixture.Context.TBL_LOJA
            .Include(l => l.Endereco)
            .Include(l => l.Telefone)
            .SingleAsync(l => l.Id == loja.Id);

        Assert.Equal(TipoDocumentoFiscalLoja.CPF, lojaSalva.TipoDocumentoFiscal);
        Assert.Equal("52998224725", lojaSalva.DocumentoFiscal);
        Assert.Equal("529.982.247-25", loja.DocumentoFiscalFormatado);
        Assert.Equal("atelie-da-ana", loja.Slug);
        Assert.NotNull(lojaSalva.Endereco);
        Assert.NotNull(lojaSalva.Telefone);
        Assert.NotEqual(enderecoUsuario.Id, lojaSalva.EnderecoId);
        Assert.NotEqual(telefoneUsuario.Id, lojaSalva.TelefoneId);
        Assert.Equal(enderecoUsuario.Cep, lojaSalva.Endereco!.Cep);
        Assert.Equal(enderecoUsuario.Cidade, lojaSalva.Endereco.Cidade);
        Assert.Equal(telefoneUsuario.NumeroE164, lojaSalva.Telefone!.NumeroE164);
        Assert.False(lojaSalva.Endereco.IsPrincipal);
        Assert.False(lojaSalva.Telefone.IsPrincipal);
    }

    [Fact]
    public async Task CriarMinhaLojaAsync_DeveCadastrarNovoEnderecoETelefoneDaLoja()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("empresa");
        var cnpj = fixture.GerarCnpjValidoParaTeste();

        var loja = await fixture.LojaService.CriarMinhaLojaAsync(
            usuario.Id,
            new LojaCriacaoDto
            {
                NomeFantasia = "Mercado do Bairro",
                TipoDocumentoFiscal = TipoDocumentoFiscalLoja.CNPJ,
                DocumentoFiscal = cnpj,
                EmailContato = "contato@mercado.com",
                NovoEnderecoLoja = new EnderecoCriacaoDto
                {
                    Cep = "13010000",
                    TipoLogradouro = TiposLogradouroBR.Rua,
                    NomeEndereco = "Rua do Comercio",
                    Numero = "200",
                    Complemento = "Sala 4",
                    Cidade = "Campinas",
                    Uf = "sp",
                    IsPrincipal = true
                },
                NovoTelefoneLoja = new TelefoneCriacaoDto
                {
                    Ddd = "19",
                    Numero = "998887766",
                    IsPrincipal = true
                }
            });

        fixture.Context.ChangeTracker.Clear();

        var lojaSalva = await fixture.Context.TBL_LOJA
            .Include(l => l.Endereco)
            .Include(l => l.Telefone)
            .SingleAsync(l => l.Id == loja.Id);

        Assert.Equal(TipoDocumentoFiscalLoja.CNPJ, lojaSalva.TipoDocumentoFiscal);
        Assert.Equal(cnpj, lojaSalva.DocumentoFiscal);
        Assert.Equal(
            DocumentoFiscalFormatter.Formatar(TipoDocumentoFiscalLoja.CNPJ, cnpj),
            loja.DocumentoFiscalFormatado);
        Assert.Equal("13010000", lojaSalva.Endereco!.Cep);
        Assert.Equal("Campinas", lojaSalva.Endereco.Cidade);
        Assert.Equal("SP", lojaSalva.Endereco.Uf);
        Assert.Equal("+5519998887766", lojaSalva.Telefone!.NumeroE164);
        Assert.False(lojaSalva.Endereco.IsPrincipal);
        Assert.False(lojaSalva.Telefone.IsPrincipal);
        Assert.Equal(lojaSalva.EnderecoId, loja.EnderecoId);
        Assert.Equal(lojaSalva.TelefoneId, loja.TelefoneId);
    }

    [Fact]
    public async Task CriarMinhaLojaAsync_DeveRejeitarDocumentoFiscalInvalido()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("doc-invalido");

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.LojaService.CriarMinhaLojaAsync(
                usuario.Id,
                new LojaCriacaoDto
                {
                    NomeFantasia = "Loja Teste",
                    TipoDocumentoFiscal = TipoDocumentoFiscalLoja.CPF,
                    DocumentoFiscal = "123.456.789-00"
                }));

        Assert.Equal("CPF invalido.", excecao.Message);
    }

    [Fact]
    public async Task CriarMinhaLojaAsync_DeveRejeitarEnderecoDoUsuarioInexistente()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("endereco-invalido");
        var telefoneUsuario = await fixture.CriarTelefoneAsync(usuario.Id);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.LojaService.CriarMinhaLojaAsync(
                usuario.Id,
                new LojaCriacaoDto
                {
                    NomeFantasia = "Loja Teste",
                    TipoDocumentoFiscal = TipoDocumentoFiscalLoja.CPF,
                    DocumentoFiscal = "529.982.247-25",
                    UsarEnderecoUsuario = true,
                    EnderecoUsuarioId = 999,
                    UsarTelefoneUsuario = true,
                    TelefoneUsuarioId = telefoneUsuario.Id
                }));

        Assert.Equal("Endereco do usuario nao encontrado.", excecao.Message);
    }

    [Fact]
    public async Task AtualizarMinhaLojaAsync_DeveTrocarEnderecoETelefonePorCopiasNovas()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("atualiza-loja");
        var lojaOriginal = await fixture.CriarLojaAsync(usuario.Id);
        var enderecoOriginalId = lojaOriginal.EnderecoId;
        var telefoneOriginalId = lojaOriginal.TelefoneId;
        var enderecoUsuario = await fixture.CriarEnderecoAsync(usuario.Id, principal: false);
        var telefoneUsuario = await fixture.CriarTelefoneAsync(usuario.Id, principal: false, numeroE164: "+5511976543210");

        var lojaAtualizada = await fixture.LojaService.AtualizarMinhaLojaAsync(
            usuario.Id,
            new LojaAtualizacaoDto
            {
                NomeFantasia = "Loja Atualizada",
                Slug = "loja-atualizada",
                TipoDocumentoFiscal = TipoDocumentoFiscalLoja.CPF,
                DocumentoFiscal = usuario.Cpf,
                EmailContato = "contato@lojaatualizada.com",
                UsarEnderecoUsuario = true,
                EnderecoUsuarioId = enderecoUsuario.Id,
                UsarTelefoneUsuario = true,
                TelefoneUsuarioId = telefoneUsuario.Id,
                Ativa = true
            });

        fixture.Context.ChangeTracker.Clear();

        var lojaSalva = await fixture.Context.TBL_LOJA
            .Include(l => l.Endereco)
            .Include(l => l.Telefone)
            .SingleAsync(l => l.Id == lojaOriginal.Id);

        Assert.NotNull(lojaAtualizada);
        Assert.Equal("Loja Atualizada", lojaSalva.NomeFantasia);
        Assert.Equal("loja-atualizada", lojaSalva.Slug);
        Assert.Equal(
            DocumentoFiscalFormatter.Formatar(TipoDocumentoFiscalLoja.CPF, usuario.Cpf),
            lojaAtualizada!.DocumentoFiscalFormatado);
        Assert.NotEqual(enderecoOriginalId, lojaSalva.EnderecoId);
        Assert.NotEqual(telefoneOriginalId, lojaSalva.TelefoneId);
        Assert.NotEqual(enderecoUsuario.Id, lojaSalva.EnderecoId);
        Assert.NotEqual(telefoneUsuario.Id, lojaSalva.TelefoneId);
        Assert.Equal(enderecoUsuario.Cep, lojaSalva.Endereco!.Cep);
        Assert.Equal(telefoneUsuario.NumeroE164, lojaSalva.Telefone!.NumeroE164);
    }

    [Fact]
    public async Task CriarMinhaLojaAsync_DeveImpedirDocumentoFiscalDuplicado()
    {
        using var fixture = new ServiceTestFixture();
        var primeiroUsuario = await fixture.CriarUsuarioAsync("lojista-um");
        var segundoUsuario = await fixture.CriarUsuarioAsync("lojista-dois");
        var cnpj = fixture.GerarCnpjValidoParaTeste();

        await fixture.LojaService.CriarMinhaLojaAsync(
            primeiroUsuario.Id,
            new LojaCriacaoDto
            {
                NomeFantasia = "Mercado Um",
                TipoDocumentoFiscal = TipoDocumentoFiscalLoja.CNPJ,
                DocumentoFiscal = cnpj,
                EmailContato = "mercado1@teste.com",
                NovoEnderecoLoja = new EnderecoCriacaoDto
                {
                    Cep = "01001000",
                    TipoLogradouro = TiposLogradouroBR.Rua,
                    NomeEndereco = "Rua A",
                    Numero = "10",
                    Cidade = "Sao Paulo",
                    Uf = "SP",
                    IsPrincipal = true
                },
                NovoTelefoneLoja = new TelefoneCriacaoDto
                {
                    Ddd = "11",
                    Numero = "999998888",
                    IsPrincipal = true
                }
            });

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.LojaService.CriarMinhaLojaAsync(
                segundoUsuario.Id,
                new LojaCriacaoDto
                {
                    NomeFantasia = "Mercado Dois",
                    TipoDocumentoFiscal = TipoDocumentoFiscalLoja.CNPJ,
                    DocumentoFiscal = cnpj,
                    EmailContato = "mercado2@teste.com",
                    NovoEnderecoLoja = new EnderecoCriacaoDto
                    {
                        Cep = "01310930",
                        TipoLogradouro = TiposLogradouroBR.Avenida,
                        NomeEndereco = "Paulista",
                        Numero = "1000",
                        Cidade = "Sao Paulo",
                        Uf = "SP",
                        IsPrincipal = true
                    },
                    NovoTelefoneLoja = new TelefoneCriacaoDto
                    {
                        Ddd = "11",
                        Numero = "988887777",
                        IsPrincipal = true
                    }
                }));

        Assert.Equal("Documento fiscal ja cadastrado.", excecao.Message);
    }

    [Fact]
    public async Task ObterMinhasMetricasAsync_DeveRetornarResumoZeradoQuandoNaoHaPedidos()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("loja-sem-vendas");
        var loja = await fixture.CriarLojaAsync(usuario.Id, nomeFantasia: "Loja sem vendas");

        var metricas = await fixture.LojaService.ObterMinhasMetricasAsync(usuario.Id);

        Assert.NotNull(metricas);
        Assert.Equal(loja.Id, metricas!.LojaId);
        Assert.Equal("Loja sem vendas", metricas.NomeFantasia);
        Assert.Equal(0m, metricas.FaturamentoBruto);
        Assert.Equal(0m, metricas.FaturamentoLiquido);
        Assert.Equal(0m, metricas.TicketMedio);
        Assert.Equal(0d, metricas.TaxaCancelamento);
        Assert.Equal(5, metricas.PedidosPorStatus.Count);
        Assert.All(metricas.PedidosPorStatus, status => Assert.Equal(0, status.Total));
        Assert.Empty(metricas.ProdutosMaisVendidosPorQuantidade);
        Assert.Empty(metricas.ProdutosMaisVendidosPorReceita);
    }

    [Fact]
    public async Task ObterMinhasMetricasAsync_DeveConsolidarFaturamentoPedidosTopProdutosEAvaliacoes()
    {
        using var fixture = new ServiceTestFixture();
        var vendedorPrincipal = await fixture.CriarUsuarioAsync("seller-principal");
        var vendedorSecundario = await fixture.CriarUsuarioAsync("seller-secundario");
        await fixture.CriarLojaAsync(vendedorPrincipal.Id, nomeFantasia: "Loja principal");
        await fixture.CriarLojaAsync(vendedorSecundario.Id, nomeFantasia: "Loja secundaria");

        var produtoPrincipalA = await fixture.CriarProdutoAsync(vendedorPrincipal.Id, preco: 20m, estoque: 20);
        var produtoPrincipalB = await fixture.CriarProdutoAsync(vendedorPrincipal.Id, preco: 50m, estoque: 20);
        var produtoSecundario = await fixture.CriarProdutoAsync(vendedorSecundario.Id, preco: 30m, estoque: 20);

        var compradorPago = await fixture.CriarUsuarioAsync("comprador-pago");
        var compradorEntregue = await fixture.CriarUsuarioAsync("comprador-entregue");
        var compradorCancelado = await fixture.CriarUsuarioAsync("comprador-cancelado");
        var compradorPendente = await fixture.CriarUsuarioAsync("comprador-pendente");

        var enderecoPago = await fixture.CriarEnderecoAsync(compradorPago.Id);
        var enderecoEntregue = await fixture.CriarEnderecoAsync(compradorEntregue.Id);
        var enderecoCancelado = await fixture.CriarEnderecoAsync(compradorCancelado.Id);
        var enderecoPendente = await fixture.CriarEnderecoAsync(compradorPendente.Id);

        var pedidoPago = await fixture.PedidoService.CriarPedido(
            compradorPago.Id,
            new PedidoDto
            {
                EnderecoId = enderecoPago.Id,
                TipoEntregaId = (int)TipoEntrega.Retirada,
                Itens =
                [
                    new ItemPedidoDto
                    {
                        ProdutoId = produtoPrincipalA.Id,
                        Quantidade = 2
                    },
                    new ItemPedidoDto
                    {
                        ProdutoId = produtoSecundario.Id,
                        Quantidade = 1
                    }
                ]
            });

        var pagamentoPedidoPago = await fixture.FinanceiroService.IniciarPagamentoAsync(
            compradorPago.Id,
            new IniciarPagamentoDto
            {
                PedidoId = pedidoPago.Id,
                FormaPagamentoId = 1
            });

        await fixture.FinanceiroService.ConfirmarPagamentoFakeAsync(
            compradorPago.Id,
            pagamentoPedidoPago.PlanoPagamentoId);

        var pedidoEntregue = await fixture.PedidoService.CriarPedido(
            compradorEntregue.Id,
            new PedidoDto
            {
                EnderecoId = enderecoEntregue.Id,
                TipoEntregaId = (int)TipoEntrega.Retirada,
                Itens =
                [
                    new ItemPedidoDto
                    {
                        ProdutoId = produtoPrincipalB.Id,
                        Quantidade = 1
                    }
                ]
            });

        var pagamentoPedidoEntregue = await fixture.FinanceiroService.IniciarPagamentoAsync(
            compradorEntregue.Id,
            new IniciarPagamentoDto
            {
                PedidoId = pedidoEntregue.Id,
                FormaPagamentoId = 1
            });

        await fixture.FinanceiroService.ConfirmarPagamentoFakeAsync(
            compradorEntregue.Id,
            pagamentoPedidoEntregue.PlanoPagamentoId);
        await fixture.PedidoService.MarcarPedidoComoEnviadoAsync(pedidoEntregue.Id);
        await fixture.PedidoService.ConfirmarEntregaPedidoAsync(pedidoEntregue.Id, compradorEntregue.Id);

        await fixture.AvaliacaoProdutoService.CriarAsync(
            produtoPrincipalB.Id,
            compradorEntregue.Id,
            new ProdutoAvaliacaoCriacaoDto
            {
                PedidoId = pedidoEntregue.Id,
                NotaProduto = 4,
                NotaLoja = 5,
                RecomendaProduto = true
            });

        var pedidoCancelado = await fixture.PedidoService.CriarPedido(
            compradorCancelado.Id,
            new PedidoDto
            {
                EnderecoId = enderecoCancelado.Id,
                TipoEntregaId = (int)TipoEntrega.Retirada,
                Itens =
                [
                    new ItemPedidoDto
                    {
                        ProdutoId = produtoPrincipalA.Id,
                        Quantidade = 1
                    }
                ]
            });

        await fixture.PedidoService.CancelarPedido(pedidoCancelado.Id, compradorCancelado.Id);

        await fixture.PedidoService.CriarPedido(
            compradorPendente.Id,
            new PedidoDto
            {
                EnderecoId = enderecoPendente.Id,
                TipoEntregaId = (int)TipoEntrega.Retirada,
                Itens =
                [
                    new ItemPedidoDto
                    {
                        ProdutoId = produtoPrincipalA.Id,
                        Quantidade = 1
                    }
                ]
            });

        fixture.Context.ChangeTracker.Clear();

        var metricas = await fixture.LojaService.ObterMinhasMetricasAsync(vendedorPrincipal.Id);

        Assert.NotNull(metricas);
        Assert.Equal(90m, metricas!.FaturamentoBruto);
        Assert.Equal(90m, metricas.FaturamentoLiquido);
        Assert.Equal(45m, metricas.TicketMedio);
        Assert.Equal(25d, metricas.TaxaCancelamento);
        Assert.Equal(5d, metricas.MediaAvaliacao);
        Assert.Equal(1, metricas.TotalAvaliacoes);

        Assert.Equal(1, metricas.PedidosPorStatus.Single(x => x.Status == StatusPedido.Pendente).Total);
        Assert.Equal(1, metricas.PedidosPorStatus.Single(x => x.Status == StatusPedido.Pago).Total);
        Assert.Equal(0, metricas.PedidosPorStatus.Single(x => x.Status == StatusPedido.Enviado).Total);
        Assert.Equal(1, metricas.PedidosPorStatus.Single(x => x.Status == StatusPedido.Entregue).Total);
        Assert.Equal(1, metricas.PedidosPorStatus.Single(x => x.Status == StatusPedido.Cancelado).Total);

        var topQuantidade = metricas.ProdutosMaisVendidosPorQuantidade;
        Assert.Equal(2, topQuantidade.Count);
        Assert.Equal(produtoPrincipalA.Id, topQuantidade[0].ProdutoId);
        Assert.Equal(2, topQuantidade[0].QuantidadeVendida);
        Assert.Equal(40m, topQuantidade[0].ReceitaBruta);
        Assert.Equal(produtoPrincipalB.Id, topQuantidade[1].ProdutoId);

        var topReceita = metricas.ProdutosMaisVendidosPorReceita;
        Assert.Equal(2, topReceita.Count);
        Assert.Equal(produtoPrincipalB.Id, topReceita[0].ProdutoId);
        Assert.Equal(50m, topReceita[0].ReceitaBruta);
        Assert.Equal(produtoPrincipalA.Id, topReceita[1].ProdutoId);
    }
}
