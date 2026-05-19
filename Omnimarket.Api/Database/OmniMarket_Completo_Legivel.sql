/* ============================================================
   OMNIMARKET - SCRIPT COMPLETO LEGIVEL
   ============================================================

   Objetivo: criar o banco do cenario atual da API de forma
   mais facil de ler e explicar em apresentacao.

   Este script esta dividido em:
   1) tabelas principais;
   2) dados iniciais;
   3) indices criados pelo modelo;
   4) indices extras, constraints, procedures e triggers.

   Observacao: as PRIMARY KEYs do SQL Server sao CLUSTERED
   por padrao quando nao informamos NONCLUSTERED.
*/


/* ------------------------------------------------------------
   Tabela: TBL_CONDICAO_PAGAMENTO
   Guarda as condicoes de pagamento, como a vista, 2x ou 3x sem juros.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_CONDICAO_PAGAMENTO] (
    [Id] int NOT NULL IDENTITY,
    [Nome] varchar(200) NOT NULL,
    [QuantidadeParcelas] int NOT NULL,
    [IntervaloDias] int NOT NULL,
    [PossuiJuros] bit NOT NULL,
    [PercentualJuros] decimal(18,2) NOT NULL,
    [ValorEntrada] decimal(18,2) NOT NULL,
    [Ativa] bit NOT NULL,
    [Observacao] varchar(200) NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_CONDICAO_PAGAMENTO] PRIMARY KEY ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_FORMA_PAGAMENTO
   Guarda as formas de pagamento aceitas, como Pix, dinheiro, debito, credito e vales.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_FORMA_PAGAMENTO] (
    [Id] int NOT NULL IDENTITY,
    [Nome] varchar(200) NOT NULL,
    [Ativo] bit NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    [Observacao] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_FORMA_PAGAMENTO] PRIMARY KEY ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_TERMO_POLITICA
   Guarda termos e politicas que o usuario ou vendedor pode aceitar.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_TERMO_POLITICA] (
    [Id] int NOT NULL IDENTITY,
    [Codigo] varchar(200) NOT NULL,
    [Versao] varchar(200) NOT NULL,
    [Resumo] varchar(200) NOT NULL,
    [Texto] varchar(200) NOT NULL,
    [Ativo] bit NOT NULL,
    [DataPublicacao] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_TERMO_POLITICA] PRIMARY KEY ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_USUARIO
   Guarda compradores, vendedores e administradores do sistema.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_USUARIO] (
    [Id] int NOT NULL IDENTITY,
    [Cpf] varchar(200) NOT NULL,
    [Nome] varchar(200) NOT NULL,
    [Sobrenome] varchar(200) NOT NULL,
    [PasswordHash] varbinary(max) NOT NULL,
    [PasswordSalt] varbinary(max) NOT NULL,
    [Foto] varbinary(max) NULL,
    [DataAcesso] datetime2 NULL,
    [DataCadastro] datetime2 NOT NULL,
    [Email] varchar(200) NOT NULL,
    [AceitouTermos] bit NOT NULL,
    [DataAceiteTermos] datetime2 NULL,
    [Role] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_USUARIO] PRIMARY KEY ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_ACEITE_USUARIO_TERMO
   Registra quando um usuario aceitou uma versao de termo ou politica.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_ACEITE_USUARIO_TERMO] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [TermoPoliticaId] int NOT NULL,
    [VersaoTermo] varchar(200) NOT NULL,
    [ResumoAceito] varchar(200) NOT NULL,
    [IdentificadorAceite] varchar(200) NOT NULL,
    [DataAceite] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_ACEITE_USUARIO_TERMO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ACEITE_USUARIO_TERMO_TBL_TERMO_POLITICA_TermoPoliticaId] FOREIGN KEY ([TermoPoliticaId]) REFERENCES [TBL_TERMO_POLITICA] ([Id]),
    CONSTRAINT [FK_TBL_ACEITE_USUARIO_TERMO_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_CARRINHO
   Representa o carrinho de compras de um usuario.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_CARRINHO] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_TBL_CARRINHO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_CARRINHO_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_ENDERECO
   Guarda os enderecos cadastrados pelo usuario.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_ENDERECO] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [TipoLogradouro] nvarchar(max) NOT NULL,
    [NomeEndereco] varchar(200) NOT NULL,
    [Numero] varchar(200) NOT NULL,
    [Complemento] varchar(200) NULL,
    [Cep] varchar(200) NOT NULL,
    [Cidade] varchar(200) NOT NULL,
    [Uf] varchar(200) NOT NULL,
    [IsPrincipal] bit NOT NULL,
    CONSTRAINT [PK_TBL_ENDERECO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ENDERECO_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_LOJA
   Guarda a loja/vitrine do usuario vendedor.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_LOJA] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [NomeFantasia] varchar(200) NOT NULL,
    [Slug] varchar(200) NOT NULL,
    [Descricao] varchar(200) NULL,
    [EmailContato] varchar(200) NULL,
    [TelefoneContato] varchar(200) NULL,
    [Cidade] varchar(200) NULL,
    [Uf] varchar(200) NULL,
    [Ativa] bit NOT NULL,
    [DtCriacao] datetimeoffset NOT NULL,
    [DtAtualizacao] datetimeoffset NULL,
    CONSTRAINT [PK_TBL_LOJA] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_LOJA_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_PEDIDO
   Guarda o pedido principal feito pelo comprador.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_PEDIDO] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [TipoLogradouroEntrega] varchar(200) NOT NULL,
    [NomeEnderecoEntrega] varchar(200) NOT NULL,
    [NumeroEntrega] varchar(200) NOT NULL,
    [ComplementoEntrega] varchar(200) NULL,
    [CepEntrega] varchar(200) NOT NULL,
    [CidadeEntrega] varchar(200) NOT NULL,
    [UfEntrega] varchar(200) NOT NULL,
    [TipoEntregaId] int NOT NULL,
    [StatusPedidosId] int NOT NULL,
    [ValorTotalProdutos] decimal(18,2) NOT NULL,
    [ValorFrete] decimal(18,2) NOT NULL,
    [ValorTotalPedido] decimal(18,2) NOT NULL,
    [DataPedido] datetime2 NOT NULL,
    [Observacao] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_PEDIDO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PEDIDO_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_PRODUTOS
   Guarda os produtos anunciados pelos vendedores.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_PRODUTOS] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [Nome] varchar(200) NOT NULL,
    [Categoria] varchar(200) NOT NULL,
    [Sku] varchar(200) NOT NULL,
    [Preco] decimal(18,2) NOT NULL,
    [Estoque] int NOT NULL,
    [Descricao] varchar(200) NULL,
    [StatusPublicacao] nvarchar(max) NOT NULL,
    [MediaAvaliacao] float NOT NULL,
    [TotalAvaliacoes] int NOT NULL,
    [DtCriacao] datetimeoffset NOT NULL,
    [DtAtualizacao] datetimeoffset NULL,
    CONSTRAINT [PK_TBL_PRODUTOS] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PRODUTOS_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_TELEFONE
   Guarda os telefones vinculados ao usuario.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_TELEFONE] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [NumeroE164] varchar(200) NOT NULL,
    [IsPrincipal] bit NOT NULL,
    CONSTRAINT [PK_TBL_TELEFONE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_TELEFONE_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_USUARIO_CARTAO_PAGAMENTO
   Guarda os cartoes tokenizados do usuario, sem numero completo e sem CVV.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_USUARIO_CARTAO_PAGAMENTO] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [Tipo] nvarchar(max) NOT NULL,
    [Apelido] varchar(200) NOT NULL,
    [Bandeira] varchar(200) NOT NULL,
    [Ultimos4Digitos] varchar(200) NOT NULL,
    [TokenGateway] varchar(200) NOT NULL,
    [MesValidade] int NOT NULL,
    [AnoValidade] int NOT NULL,
    [Ativo] bit NOT NULL,
    [IsPrincipal] bit NOT NULL,
    [DataCadastro] datetime2 NOT NULL,
    [DataAtualizacao] datetime2 NULL,
    CONSTRAINT [PK_TBL_USUARIO_CARTAO_PAGAMENTO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_USUARIO_CARTAO_PAGAMENTO_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_PLANO_PAGAMENTO
   Guarda o plano de pagamento geral de um pedido.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_PLANO_PAGAMENTO] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [ValorTotal] decimal(18,2) NOT NULL,
    [StatusPagamento] nvarchar(max) NOT NULL,
    [GatewayTransactionId] varchar(200) NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    [DataConfirmacao] datetime2 NULL,
    [Observacoes] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_PLANO_PAGAMENTO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PLANO_PAGAMENTO_TBL_PEDIDO_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [TBL_PEDIDO] ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_VENDA
   Guarda a venda separada por vendedor dentro de um pedido.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_VENDA] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [VendedorId] int NOT NULL,
    [ValorBruto] decimal(18,2) NOT NULL,
    [ValorComissao] decimal(18,2) NOT NULL,
    [ValorLiquido] decimal(18,2) NOT NULL,
    [ValorFrete] decimal(18,2) NOT NULL,
    [ValorFixoComissaoAplicado] decimal(18,2) NOT NULL,
    [PercentualComissaoAplicado] decimal(18,2) NOT NULL,
    [FormulaComissaoAplicada] varchar(200) NOT NULL,
    [StatusVenda] nvarchar(max) NOT NULL,
    [StatusFinanceiro] nvarchar(max) NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    [DataAtualizacao] datetime2 NULL,
    CONSTRAINT [PK_TBL_VENDA] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_VENDA_TBL_PEDIDO_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [TBL_PEDIDO] ([Id]),
    CONSTRAINT [FK_TBL_VENDA_TBL_USUARIO_VendedorId] FOREIGN KEY ([VendedorId]) REFERENCES [TBL_USUARIO] ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_ITEM_CARRINHO
   Guarda os produtos adicionados ao carrinho.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_ITEM_CARRINHO] (
    [Id] int NOT NULL IDENTITY,
    [CarrinhoId] int NOT NULL,
    [ProdutoId] int NOT NULL,
    [Quantidade] int NOT NULL,
    CONSTRAINT [PK_TBL_ITEM_CARRINHO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ITEM_CARRINHO_TBL_CARRINHO_CarrinhoId] FOREIGN KEY ([CarrinhoId]) REFERENCES [TBL_CARRINHO] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_ITEM_CARRINHO_TBL_PRODUTOS_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [TBL_PRODUTOS] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_ITENS_PEDIDO
   Guarda os produtos comprados dentro de um pedido.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_ITENS_PEDIDO] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [ProdutoId] int NOT NULL,
    [Quantidade] int NOT NULL,
    [PrecoUnitario] decimal(18,2) NOT NULL,
    [ValorTotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_TBL_ITENS_PEDIDO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ITENS_PEDIDO_TBL_PEDIDO_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [TBL_PEDIDO] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_ITENS_PEDIDO_TBL_PRODUTOS_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [TBL_PRODUTOS] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_PRODUTOS_MIDIA
   Guarda as URLs das fotos e videos dos produtos.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_PRODUTOS_MIDIA] (
    [Id] int NOT NULL IDENTITY,
    [ProdutoId] int NOT NULL,
    [Tipo] int NOT NULL,
    [Url] varchar(200) NOT NULL,
    [ContentType] varchar(200) NULL,
    [Ordem] int NOT NULL,
    [DtCriacao] datetimeoffset NOT NULL,
    CONSTRAINT [PK_TBL_PRODUTOS_MIDIA] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PRODUTOS_MIDIA_TBL_PRODUTOS_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [TBL_PRODUTOS] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_ITEM_PLANO_PAGAMENTO
   Guarda cada parte do pagamento, como Pix + cartao.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_ITEM_PLANO_PAGAMENTO] (
    [Id] int NOT NULL IDENTITY,
    [PlanoPagamentoId] int NOT NULL,
    [FormaPagamentoId] int NOT NULL,
    [CondicaoPagamentoId] int NULL,
    [Valor] decimal(18,2) NOT NULL,
    [QuantidadeParcelas] int NOT NULL,
    [ValorParcela] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CodigoAutorizacao] varchar(200) NOT NULL,
    [Observacao] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_ITEM_PLANO_PAGAMENTO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ITEM_PLANO_PAGAMENTO_TBL_CONDICAO_PAGAMENTO_CondicaoPagamentoId] FOREIGN KEY ([CondicaoPagamentoId]) REFERENCES [TBL_CONDICAO_PAGAMENTO] ([Id]),
    CONSTRAINT [FK_TBL_ITEM_PLANO_PAGAMENTO_TBL_FORMA_PAGAMENTO_FormaPagamentoId] FOREIGN KEY ([FormaPagamentoId]) REFERENCES [TBL_FORMA_PAGAMENTO] ([Id]),
    CONSTRAINT [FK_TBL_ITEM_PLANO_PAGAMENTO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId] FOREIGN KEY ([PlanoPagamentoId]) REFERENCES [TBL_PLANO_PAGAMENTO] ([Id]) ON DELETE CASCADE
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_HISTORICO_FINANCEIRO
   Guarda a linha do tempo dos eventos financeiros.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_HISTORICO_FINANCEIRO] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NULL,
    [VendaId] int NULL,
    [PlanoPagamentoId] int NULL,
    [TipoMovimentacao] nvarchar(max) NOT NULL,
    [Descricao] varchar(200) NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    [DataMovimentacao] datetime2 NOT NULL,
    [UsuarioResponsavelId] int NULL,
    CONSTRAINT [PK_TBL_HISTORICO_FINANCEIRO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_HISTORICO_FINANCEIRO_TBL_PEDIDO_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [TBL_PEDIDO] ([Id]),
    CONSTRAINT [FK_TBL_HISTORICO_FINANCEIRO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId] FOREIGN KEY ([PlanoPagamentoId]) REFERENCES [TBL_PLANO_PAGAMENTO] ([Id]),
    CONSTRAINT [FK_TBL_HISTORICO_FINANCEIRO_TBL_USUARIO_UsuarioResponsavelId] FOREIGN KEY ([UsuarioResponsavelId]) REFERENCES [TBL_USUARIO] ([Id]),
    CONSTRAINT [FK_TBL_HISTORICO_FINANCEIRO_TBL_VENDA_VendaId] FOREIGN KEY ([VendaId]) REFERENCES [TBL_VENDA] ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_REPASSE_VENDEDOR
   Guarda o controle de repasse do valor liquido para o vendedor.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_REPASSE_VENDEDOR] (
    [Id] int NOT NULL IDENTITY,
    [VendaId] int NOT NULL,
    [VendedorId] int NOT NULL,
    [ValorRepasse] decimal(18,2) NOT NULL,
    [DataPrevistaRepasse] datetime2 NOT NULL,
    [DataEfetivaRepasse] datetime2 NULL,
    [StatusRepasse] nvarchar(max) NOT NULL,
    [GatewayPayoutId] varchar(200) NOT NULL,
    [Observacao] varchar(200) NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_REPASSE_VENDEDOR] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_REPASSE_VENDEDOR_TBL_USUARIO_VendedorId] FOREIGN KEY ([VendedorId]) REFERENCES [TBL_USUARIO] ([Id]),
    CONSTRAINT [FK_TBL_REPASSE_VENDEDOR_TBL_VENDA_VendaId] FOREIGN KEY ([VendaId]) REFERENCES [TBL_VENDA] ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_VENDA_PAGAMENTO
   Liga a venda ao plano de pagamento e ao status financeiro.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_VENDA_PAGAMENTO] (
    [Id] int NOT NULL IDENTITY,
    [VendaId] int NOT NULL,
    [PlanoPagamentoId] int NOT NULL,
    [ValorPagoConsiderado] decimal(18,2) NOT NULL,
    [ValorComissaoConsiderado] decimal(18,2) NOT NULL,
    [ValorLiquidoConsiderado] decimal(18,2) NOT NULL,
    [StatusFinanceiro] nvarchar(max) NOT NULL,
    [DataConfirmacao] datetime2 NULL,
    [Observacao] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_VENDA_PAGAMENTO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_VENDA_PAGAMENTO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId] FOREIGN KEY ([PlanoPagamentoId]) REFERENCES [TBL_PLANO_PAGAMENTO] ([Id]),
    CONSTRAINT [FK_TBL_VENDA_PAGAMENTO_TBL_VENDA_VendaId] FOREIGN KEY ([VendaId]) REFERENCES [TBL_VENDA] ([Id])
);
GO



/* ------------------------------------------------------------
   Tabela: TBL_RECIBO_FINANCEIRO
   Guarda os recibos financeiros gerados apos pagamento ou repasse.
   ------------------------------------------------------------ */
CREATE TABLE [TBL_RECIBO_FINANCEIRO] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [VendaId] int NULL,
    [PlanoPagamentoId] int NULL,
    [RepasseVendedorId] int NULL,
    [NumeroRecibo] varchar(200) NOT NULL,
    [MensagemLegal] varchar(200) NOT NULL,
    [TextoAceiteVendedor] varchar(200) NOT NULL,
    [ValorBruto] decimal(18,2) NOT NULL,
    [ValorComissao] decimal(18,2) NOT NULL,
    [ValorLiquido] decimal(18,2) NOT NULL,
    [DataGeracao] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_RECIBO_FINANCEIRO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_RECIBO_FINANCEIRO_TBL_PEDIDO_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [TBL_PEDIDO] ([Id]),
    CONSTRAINT [FK_TBL_RECIBO_FINANCEIRO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId] FOREIGN KEY ([PlanoPagamentoId]) REFERENCES [TBL_PLANO_PAGAMENTO] ([Id]),
    CONSTRAINT [FK_TBL_RECIBO_FINANCEIRO_TBL_REPASSE_VENDEDOR_RepasseVendedorId] FOREIGN KEY ([RepasseVendedorId]) REFERENCES [TBL_REPASSE_VENDEDOR] ([Id]),
    CONSTRAINT [FK_TBL_RECIBO_FINANCEIRO_TBL_VENDA_VendaId] FOREIGN KEY ([VendaId]) REFERENCES [TBL_VENDA] ([Id])
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativa', N'DataCriacao', N'IntervaloDias', N'Nome', N'Observacao', N'PercentualJuros', N'PossuiJuros', N'QuantidadeParcelas', N'ValorEntrada') AND [object_id] = OBJECT_ID(N'[TBL_CONDICAO_PAGAMENTO]'))
    SET IDENTITY_INSERT [TBL_CONDICAO_PAGAMENTO] ON;

-- Insere condicoes de pagamento padrao usadas nos testes e no MVP.
INSERT INTO [TBL_CONDICAO_PAGAMENTO] ([Id], [Ativa], [DataCriacao], [IntervaloDias], [Nome], [Observacao], [PercentualJuros], [PossuiJuros], [QuantidadeParcelas], [ValorEntrada])
VALUES (1, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 0, 'A vista', 'Pagamento integral em uma parcela.', 0.0, CAST(0 AS bit), 1, 0.0),
(2, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 30, '2x sem juros', 'Parcelamento em 2 vezes sem juros.', 0.0, CAST(0 AS bit), 2, 0.0),
(3, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 30, '3x sem juros', 'Parcelamento em 3 vezes sem juros.', 0.0, CAST(0 AS bit), 3, 0.0),
(4, CAST(0 AS bit), '2026-01-01T00:00:00.0000000Z', 30, 'Pix parcelado (futuro)', 'Condicao prevista para evolucao futura.', 2.5, CAST(1 AS bit), 2, 0.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativa', N'DataCriacao', N'IntervaloDias', N'Nome', N'Observacao', N'PercentualJuros', N'PossuiJuros', N'QuantidadeParcelas', N'ValorEntrada') AND [object_id] = OBJECT_ID(N'[TBL_CONDICAO_PAGAMENTO]'))
    SET IDENTITY_INSERT [TBL_CONDICAO_PAGAMENTO] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'DataCriacao', N'Nome', N'Observacao') AND [object_id] = OBJECT_ID(N'[TBL_FORMA_PAGAMENTO]'))
    SET IDENTITY_INSERT [TBL_FORMA_PAGAMENTO] ON;

-- Insere formas de pagamento padrao, incluindo cartao de credito, debito e vales.
INSERT INTO [TBL_FORMA_PAGAMENTO] ([Id], [Ativo], [DataCriacao], [Nome], [Observacao])
VALUES (1, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 'Pix', 'Pagamento instantaneo via Pix.'),
(2, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 'Dinheiro', 'Pagamento em dinheiro na retirada.'),
(3, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 'Cartao de Debito', 'Pagamento em cartao de debito.'),
(4, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 'Cartao de Credito', 'Pagamento em cartao de credito.'),
(5, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 'Vale Refeicao', 'Pagamento com vale refeicao.'),
(6, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', 'Vale Alimentacao', 'Pagamento com vale alimentacao.');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'DataCriacao', N'Nome', N'Observacao') AND [object_id] = OBJECT_ID(N'[TBL_FORMA_PAGAMENTO]'))
    SET IDENTITY_INSERT [TBL_FORMA_PAGAMENTO] OFF;
GO



-- Garante que o comprovante de aceite seja unico.
CREATE UNIQUE INDEX [IX_TBL_ACEITE_USUARIO_TERMO_IdentificadorAceite] ON [TBL_ACEITE_USUARIO_TERMO] ([IdentificadorAceite]);
GO



-- Ajuda a buscar todos os aceites de um termo.
CREATE INDEX [IX_TBL_ACEITE_USUARIO_TERMO_TermoPoliticaId] ON [TBL_ACEITE_USUARIO_TERMO] ([TermoPoliticaId]);
GO



-- Ajuda a buscar todos os termos aceitos por um usuario.
CREATE INDEX [IX_TBL_ACEITE_USUARIO_TERMO_UsuarioId] ON [TBL_ACEITE_USUARIO_TERMO] ([UsuarioId]);
GO



-- Garante que cada usuario tenha somente um carrinho.
CREATE UNIQUE INDEX [IX_TBL_CARRINHO_UsuarioId] ON [TBL_CARRINHO] ([UsuarioId]);
GO



-- Ajuda a listar os enderecos de um usuario.
CREATE INDEX [IX_TBL_ENDERECO_UsuarioId] ON [TBL_ENDERECO] ([UsuarioId]);
GO



-- Evita cadastrar duas formas gerais de pagamento com o mesmo nome.
CREATE UNIQUE INDEX [IX_TBL_FORMA_PAGAMENTO_Nome] ON [TBL_FORMA_PAGAMENTO] ([Nome]);
GO



-- Ajuda a consultar historico financeiro por pedido.
CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_PedidoId] ON [TBL_HISTORICO_FINANCEIRO] ([PedidoId]);
GO



-- Ajuda a consultar historico financeiro por plano de pagamento.
CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_PlanoPagamentoId] ON [TBL_HISTORICO_FINANCEIRO] ([PlanoPagamentoId]);
GO



-- Ajuda a consultar eventos feitos por um usuario responsavel.
CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_UsuarioResponsavelId] ON [TBL_HISTORICO_FINANCEIRO] ([UsuarioResponsavelId]);
GO



-- Ajuda a consultar historico financeiro por venda.
CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_VendaId] ON [TBL_HISTORICO_FINANCEIRO] ([VendaId]);
GO



-- Ajuda a carregar os itens de um carrinho.
CREATE INDEX [IX_TBL_ITEM_CARRINHO_CarrinhoId] ON [TBL_ITEM_CARRINHO] ([CarrinhoId]);
GO



-- Ajuda a encontrar em quais carrinhos um produto esta.
CREATE INDEX [IX_TBL_ITEM_CARRINHO_ProdutoId] ON [TBL_ITEM_CARRINHO] ([ProdutoId]);
GO



-- Ajuda a relacionar itens com a condicao de pagamento.
CREATE INDEX [IX_TBL_ITEM_PLANO_PAGAMENTO_CondicaoPagamentoId] ON [TBL_ITEM_PLANO_PAGAMENTO] ([CondicaoPagamentoId]);
GO



-- Ajuda a relacionar itens com a forma de pagamento.
CREATE INDEX [IX_TBL_ITEM_PLANO_PAGAMENTO_FormaPagamentoId] ON [TBL_ITEM_PLANO_PAGAMENTO] ([FormaPagamentoId]);
GO



-- Ajuda a carregar os itens de um plano de pagamento.
CREATE INDEX [IX_TBL_ITEM_PLANO_PAGAMENTO_PlanoPagamentoId] ON [TBL_ITEM_PLANO_PAGAMENTO] ([PlanoPagamentoId]);
GO



-- Ajuda a carregar os itens de um pedido.
CREATE INDEX [IX_TBL_ITENS_PEDIDO_PedidoId] ON [TBL_ITENS_PEDIDO] ([PedidoId]);
GO



-- Ajuda a encontrar pedidos que usaram determinado produto.
CREATE INDEX [IX_TBL_ITENS_PEDIDO_ProdutoId] ON [TBL_ITENS_PEDIDO] ([ProdutoId]);
GO



-- Garante que cada loja tenha uma URL/slug unico.
CREATE UNIQUE INDEX [IX_TBL_LOJA_Slug] ON [TBL_LOJA] ([Slug]);
GO



-- Garante que cada usuario tenha no maximo uma loja.
CREATE UNIQUE INDEX [IX_TBL_LOJA_UsuarioId] ON [TBL_LOJA] ([UsuarioId]);
GO



-- Ajuda a listar pedidos de um usuario.
CREATE INDEX [IX_TBL_PEDIDO_UsuarioId] ON [TBL_PEDIDO] ([UsuarioId]);
GO



-- Garante um plano de pagamento ativo por pedido.
CREATE UNIQUE INDEX [IX_TBL_PLANO_PAGAMENTO_PedidoId] ON [TBL_PLANO_PAGAMENTO] ([PedidoId]);
GO



-- Garante que o SKU do produto seja unico.
CREATE UNIQUE INDEX [IX_TBL_PRODUTOS_Sku] ON [TBL_PRODUTOS] ([Sku]);
GO



-- Ajuda a listar produtos de um vendedor.
CREATE INDEX [IX_TBL_PRODUTOS_UsuarioId] ON [TBL_PRODUTOS] ([UsuarioId]);
GO



-- Ajuda a carregar fotos e videos de um produto.
CREATE INDEX [IX_TBL_PRODUTOS_MIDIA_ProdutoId] ON [TBL_PRODUTOS_MIDIA] ([ProdutoId]);
GO



-- Ajuda a consultar recibos por pedido.
CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_PedidoId] ON [TBL_RECIBO_FINANCEIRO] ([PedidoId]);
GO



-- Ajuda a consultar recibos por plano de pagamento.
CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_PlanoPagamentoId] ON [TBL_RECIBO_FINANCEIRO] ([PlanoPagamentoId]);
GO



-- Ajuda a consultar recibos por repasse.
CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_RepasseVendedorId] ON [TBL_RECIBO_FINANCEIRO] ([RepasseVendedorId]);
GO



-- Ajuda a consultar recibos por venda.
CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_VendaId] ON [TBL_RECIBO_FINANCEIRO] ([VendaId]);
GO



-- Ajuda a consultar repasse a partir de uma venda.
CREATE INDEX [IX_TBL_REPASSE_VENDEDOR_VendaId] ON [TBL_REPASSE_VENDEDOR] ([VendaId]);
GO



-- Ajuda a listar repasses de um vendedor.
CREATE INDEX [IX_TBL_REPASSE_VENDEDOR_VendedorId] ON [TBL_REPASSE_VENDEDOR] ([VendedorId]);
GO



-- Ajuda a listar telefones de um usuario.
CREATE INDEX [IX_TBL_TELEFONE_UsuarioId] ON [TBL_TELEFONE] ([UsuarioId]);
GO



-- Garante que nao existam dois usuarios com o mesmo CPF.
CREATE UNIQUE INDEX [IX_TBL_USUARIO_Cpf] ON [TBL_USUARIO] ([Cpf]);
GO



-- Garante que nao existam dois usuarios com o mesmo email.
CREATE UNIQUE INDEX [IX_TBL_USUARIO_Email] ON [TBL_USUARIO] ([Email]);
GO



-- Garante que o mesmo cartao tokenizado nao seja cadastrado duas vezes.
CREATE UNIQUE INDEX [IX_TBL_USUARIO_CARTAO_PAGAMENTO_TokenGateway] ON [TBL_USUARIO_CARTAO_PAGAMENTO] ([TokenGateway]);
GO



-- Garante que cada usuario tenha no maximo um cartao principal.
CREATE UNIQUE INDEX [IX_TBL_USUARIO_CARTAO_PAGAMENTO_UsuarioId_IsPrincipal] ON [TBL_USUARIO_CARTAO_PAGAMENTO] ([UsuarioId], [IsPrincipal]) WHERE [IsPrincipal] = 1;
GO



-- Ajuda a listar as vendas geradas por um pedido.
CREATE INDEX [IX_TBL_VENDA_PedidoId] ON [TBL_VENDA] ([PedidoId]);
GO



-- Ajuda a listar vendas de um vendedor.
CREATE INDEX [IX_TBL_VENDA_VendedorId] ON [TBL_VENDA] ([VendedorId]);
GO



-- Ajuda a consultar pagamentos por plano.
CREATE INDEX [IX_TBL_VENDA_PAGAMENTO_PlanoPagamentoId] ON [TBL_VENDA_PAGAMENTO] ([PlanoPagamentoId]);
GO



-- Ajuda a consultar pagamentos por venda.
CREATE INDEX [IX_TBL_VENDA_PAGAMENTO_VendaId] ON [TBL_VENDA_PAGAMENTO] ([VendaId]);
GO




/* ============================================================
   PARTE EXTRA
   ============================================================
   A partir daqui entram objetos manuais para reforcar consultas,
   integridade e regras de negocio importantes para o TCC.
*/

/* ============================================================
   OMNIMARKET - OBJETOS EXTRAS PARA O CENARIO ATUAL
   ============================================================

   Observacao sobre clustered index:
   As chaves primarias criadas acima pelo Entity Framework viram
   indices CLUSTERED por padrao no SQL Server, porque foram criadas
   como PRIMARY KEY sem informar NONCLUSTERED.

   As tabelas principais seguem esse padrao:
   - TBL_USUARIO.Id
   - TBL_PRODUTOS.Id
   - TBL_PEDIDO.Id
   - TBL_ITENS_PEDIDO.Id
   - TBL_VENDA.Id
   - TBL_PLANO_PAGAMENTO.Id
   - TBL_REPASSE_VENDEDOR.Id
   - TBL_RECIBO_FINANCEIRO.Id

   Abaixo ficam indices extras, procedures e triggers para ajudar
   em consultas e proteger algumas regras de integridade.
*/

/* ============================================================
   INDICES EXTRAS
   ============================================================ */

-- Ajuda na busca publica de produtos por categoria, preco, estoque e nome.
-- Esse e o cenario usado pelo catalogo da loja.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_PRODUTOS_CatalogoPublico')
BEGIN
    CREATE INDEX [IX_OMNI_PRODUTOS_CatalogoPublico]
    ON [TBL_PRODUTOS] ([Categoria], [Preco], [Estoque], [Nome])
    INCLUDE ([Id], [UsuarioId], [Sku]);
END;
GO

-- Ajuda a listar os pedidos de um usuario em ordem dos mais recentes.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_PEDIDO_Usuario_Data')
BEGIN
    CREATE INDEX [IX_OMNI_PEDIDO_Usuario_Data]
    ON [TBL_PEDIDO] ([UsuarioId], [DataPedido] DESC)
    INCLUDE ([StatusPedidosId], [ValorTotalPedido], [CidadeEntrega], [UfEntrega]);
END;
GO

-- Ajuda a encontrar rapidamente em quais pedidos um produto apareceu.
-- Isso e importante porque produto desativado continua existindo no historico.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_ITENS_PEDIDO_Produto_Pedido')
BEGIN
    CREATE INDEX [IX_OMNI_ITENS_PEDIDO_Produto_Pedido]
    ON [TBL_ITENS_PEDIDO] ([ProdutoId], [PedidoId])
    INCLUDE ([Quantidade], [PrecoUnitario], [ValorTotal]);
END;
GO

-- Ajuda a carregar as fotos e videos de um produto na ordem correta.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_PRODUTOS_MIDIA_Produto_Ordem')
BEGIN
    CREATE INDEX [IX_OMNI_PRODUTOS_MIDIA_Produto_Ordem]
    ON [TBL_PRODUTOS_MIDIA] ([ProdutoId], [Ordem])
    INCLUDE ([Tipo], [Url], [ContentType]);
END;
GO

-- Evita que o mesmo produto seja duplicado no mesmo carrinho.
-- Se o usuario adicionar de novo, a API deve atualizar a quantidade.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_ITEM_CARRINHO_Unico')
BEGIN
    CREATE UNIQUE INDEX [IX_OMNI_ITEM_CARRINHO_Unico]
    ON [TBL_ITEM_CARRINHO] ([CarrinhoId], [ProdutoId]);
END;
GO

-- Garante uma venda por vendedor dentro do mesmo pedido.
-- Isso combina com a regra do marketplace: um pedido pode virar varias vendas.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_VENDA_Pedido_Vendedor_Unico')
BEGIN
    CREATE UNIQUE INDEX [IX_OMNI_VENDA_Pedido_Vendedor_Unico]
    ON [TBL_VENDA] ([PedidoId], [VendedorId]);
END;
GO

-- Ajuda o vendedor a consultar seus repasses por data prevista.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_REPASSE_Vendedor_Data')
BEGIN
    CREATE INDEX [IX_OMNI_REPASSE_Vendedor_Data]
    ON [TBL_REPASSE_VENDEDOR] ([VendedorId], [DataPrevistaRepasse])
    INCLUDE ([VendaId], [ValorRepasse], [DataEfetivaRepasse]);
END;
GO

-- Ajuda auditoria e acompanhamento dos eventos financeiros mais recentes.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_HISTORICO_Data')
BEGIN
    CREATE INDEX [IX_OMNI_HISTORICO_Data]
    ON [TBL_HISTORICO_FINANCEIRO] ([DataMovimentacao] DESC)
    INCLUDE ([PedidoId], [VendaId], [PlanoPagamentoId], [Valor]);
END;
GO

-- Garante que cada recibo tenha um numero unico.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OMNI_RECIBO_Numero')
BEGIN
    CREATE UNIQUE INDEX [IX_OMNI_RECIBO_Numero]
    ON [TBL_RECIBO_FINANCEIRO] ([NumeroRecibo]);
END;
GO

/* ============================================================
   CHECK CONSTRAINTS SIMPLES
   ============================================================ */

-- Protege produto contra preco invalido e estoque negativo.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_OMNI_PRODUTOS_Preco_Estoque')
BEGIN
    ALTER TABLE [TBL_PRODUTOS]
    ADD CONSTRAINT [CK_OMNI_PRODUTOS_Preco_Estoque]
    CHECK ([Preco] > 0 AND [Estoque] >= 0);
END;
GO

-- Protege item de pedido contra quantidade ou valor invalido.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_OMNI_ITENS_PEDIDO_Valores')
BEGIN
    ALTER TABLE [TBL_ITENS_PEDIDO]
    ADD CONSTRAINT [CK_OMNI_ITENS_PEDIDO_Valores]
    CHECK ([Quantidade] > 0 AND [PrecoUnitario] > 0 AND [ValorTotal] >= 0);
END;
GO

-- Protege o pedido contra totais negativos.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_OMNI_PEDIDO_Valores')
BEGIN
    ALTER TABLE [TBL_PEDIDO]
    ADD CONSTRAINT [CK_OMNI_PEDIDO_Valores]
    CHECK ([ValorTotalProdutos] >= 0 AND [ValorFrete] >= 0 AND [ValorTotalPedido] >= 0);
END;
GO

-- Protege venda contra comissao, liquido e frete negativos.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_OMNI_VENDA_Valores')
BEGIN
    ALTER TABLE [TBL_VENDA]
    ADD CONSTRAINT [CK_OMNI_VENDA_Valores]
    CHECK ([ValorBruto] > 0 AND [ValorComissao] >= 0 AND [ValorLiquido] >= 0 AND [ValorFrete] >= 0);
END;
GO

-- Protege plano de pagamento contra valor total zerado ou negativo.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_OMNI_PLANO_PAGAMENTO_Valor')
BEGIN
    ALTER TABLE [TBL_PLANO_PAGAMENTO]
    ADD CONSTRAINT [CK_OMNI_PLANO_PAGAMENTO_Valor]
    CHECK ([ValorTotal] > 0);
END;
GO

-- Protege cada item do plano de pagamento contra valor e parcela invalidos.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_OMNI_ITEM_PLANO_PAGAMENTO_Valores')
BEGIN
    ALTER TABLE [TBL_ITEM_PLANO_PAGAMENTO]
    ADD CONSTRAINT [CK_OMNI_ITEM_PLANO_PAGAMENTO_Valores]
    CHECK ([Valor] > 0 AND [QuantidadeParcelas] > 0 AND [ValorParcela] > 0);
END;
GO

-- Protege os cartoes tokenizados do usuario.
-- Nao guarda numero completo: apenas token do gateway e ultimos 4 digitos.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_OMNI_USUARIO_CARTAO_Dados')
BEGIN
    ALTER TABLE [TBL_USUARIO_CARTAO_PAGAMENTO]
    ADD CONSTRAINT [CK_OMNI_USUARIO_CARTAO_Dados]
    CHECK (
        [Tipo] IN (N'Credito', N'Debito', N'ValeRefeicao', N'ValeAlimentacao')
        AND [MesValidade] BETWEEN 1 AND 12
        AND [AnoValidade] BETWEEN 2026 AND 2100
        AND LEN([Ultimos4Digitos]) = 4
    );
END;
GO

/* ============================================================
   PROCEDURES
   ============================================================ */

/* 
   usp_OmniProdutoCatalogo
   Mostra os produtos que aparecem para o publico.
   Usa filtros opcionais e paginacao simples, igual ao endpoint de catalogo.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniProdutoCatalogo]
    @Nome varchar(200) = NULL,
    @Categoria varchar(200) = NULL,
    @MinPreco decimal(18,2) = NULL,
    @MaxPreco decimal(18,2) = NULL,
    @Page int = 1,
    @PageSize int = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 10;
    IF @PageSize > 50 SET @PageSize = 50;

    SELECT
        p.[Id],
        p.[UsuarioId],
        p.[Nome],
        p.[Categoria],
        p.[Sku],
        p.[Preco],
        p.[Estoque],
        p.[Descricao],
        p.[MediaAvaliacao],
        p.[TotalAvaliacoes],
        p.[DtCriacao],
        p.[DtAtualizacao]
    FROM [TBL_PRODUTOS] p
    WHERE
        p.[StatusPublicacao] = N'Publicado'
        AND p.[Estoque] > 0
        AND (@Nome IS NULL OR p.[Nome] LIKE '%' + @Nome + '%')
        AND (@Categoria IS NULL OR p.[Categoria] = @Categoria)
        AND (@MinPreco IS NULL OR p.[Preco] >= @MinPreco)
        AND (@MaxPreco IS NULL OR p.[Preco] <= @MaxPreco)
    ORDER BY p.[Nome]
    OFFSET ((@Page - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

/*
   usp_OmniProdutoAtualizarDados
   Atualiza somente preco e descricao do produto.
   Nome, categoria e SKU ficam travados para manter historico e integridade.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniProdutoAtualizarDados]
    @ProdutoId int,
    @UsuarioId int,
    @Preco decimal(18,2) = NULL,
    @Descricao varchar(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Preco IS NULL AND @Descricao IS NULL
        THROW 51000, 'Informe preco ou descricao para atualizar.', 1;

    IF @Preco IS NOT NULL AND @Preco <= 0
        THROW 51001, 'Preco deve ser maior que zero.', 1;

    UPDATE [TBL_PRODUTOS]
    SET
        [Preco] = COALESCE(@Preco, [Preco]),
        [Descricao] = CASE WHEN @Descricao IS NULL THEN [Descricao] ELSE LTRIM(RTRIM(@Descricao)) END,
        [DtAtualizacao] = SYSDATETIMEOFFSET()
    WHERE
        [Id] = @ProdutoId
        AND [UsuarioId] = @UsuarioId
        AND [StatusPublicacao] <> N'Desativado';

    IF @@ROWCOUNT = 0
        THROW 51002, 'Produto nao encontrado, nao pertence ao usuario ou esta desativado.', 1;
END;
GO

/*
   usp_OmniProdutoAtualizarEstoque
   Atualiza apenas o estoque em um metodo separado.
   Isso deixa claro que estoque nao faz parte da edicao normal do produto.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniProdutoAtualizarEstoque]
    @ProdutoId int,
    @UsuarioId int,
    @Estoque int
AS
BEGIN
    SET NOCOUNT ON;

    IF @Estoque < 0
        THROW 51003, 'Estoque nao pode ser negativo.', 1;

    UPDATE [TBL_PRODUTOS]
    SET
        [Estoque] = @Estoque,
        [DtAtualizacao] = SYSDATETIMEOFFSET()
    WHERE
        [Id] = @ProdutoId
        AND [UsuarioId] = @UsuarioId
        AND [StatusPublicacao] <> N'Desativado';

    IF @@ROWCOUNT = 0
        THROW 51004, 'Produto nao encontrado, nao pertence ao usuario ou esta desativado.', 1;
END;
GO

/*
   usp_OmniProdutoDesativar
   Desativa o produto sem apagar do banco.
   Assim pedidos antigos continuam conseguindo encontrar o produto vendido.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniProdutoDesativar]
    @ProdutoId int,
    @UsuarioId int
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [TBL_PRODUTOS]
    SET
        [StatusPublicacao] = N'Desativado',
        [DtAtualizacao] = SYSDATETIMEOFFSET()
    WHERE
        [Id] = @ProdutoId
        AND [UsuarioId] = @UsuarioId;

    IF @@ROWCOUNT = 0
        THROW 51005, 'Produto nao encontrado ou nao pertence ao usuario.', 1;
END;
GO

/*
   usp_OmniPedidoDetalhe
   Retorna duas consultas:
   1) os dados principais do pedido;
   2) os itens do pedido com dados basicos do produto.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniPedidoDetalhe]
    @PedidoId int,
    @UsuarioId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.[Id] AS PedidoId,
        p.[UsuarioId],
        p.[StatusPedidosId],
        p.[ValorTotalProdutos],
        p.[ValorFrete],
        p.[ValorTotalPedido],
        p.[DataPedido],
        p.[CidadeEntrega],
        p.[UfEntrega],
        p.[Observacao]
    FROM [TBL_PEDIDO] p
    WHERE p.[Id] = @PedidoId AND p.[UsuarioId] = @UsuarioId;

    SELECT
        i.[Id] AS ItemPedidoId,
        i.[ProdutoId],
        pr.[Nome] AS ProdutoNome,
        pr.[Sku],
        pr.[StatusPublicacao],
        i.[Quantidade],
        i.[PrecoUnitario],
        i.[ValorTotal]
    FROM [TBL_ITENS_PEDIDO] i
    INNER JOIN [TBL_PRODUTOS] pr ON pr.[Id] = i.[ProdutoId]
    WHERE i.[PedidoId] = @PedidoId
    ORDER BY i.[Id];
END;
GO

/*
   usp_OmniVendedorExtrato
   Lista vendas e repasses de um vendedor.
   E o cenario usado para mostrar quanto o vendedor vendeu e quanto vai receber.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniVendedorExtrato]
    @VendedorId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.[Id] AS VendaId,
        v.[PedidoId],
        v.[ValorBruto],
        v.[ValorComissao],
        v.[ValorLiquido],
        v.[StatusVenda],
        v.[StatusFinanceiro],
        v.[DataCriacao],
        r.[Id] AS RepasseId,
        r.[ValorRepasse],
        r.[DataPrevistaRepasse],
        r.[DataEfetivaRepasse],
        r.[StatusRepasse]
    FROM [TBL_VENDA] v
    LEFT JOIN [TBL_REPASSE_VENDEDOR] r ON r.[VendaId] = v.[Id]
    WHERE v.[VendedorId] = @VendedorId
    ORDER BY v.[DataCriacao] DESC;
END;
GO

/*
   usp_OmniResumoComissoes
   Soma o financeiro da plataforma.
   Mostra bruto vendido, comissao da plataforma e liquido dos vendedores.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniResumoComissoes]
    @DataInicio datetime2 = NULL,
    @DataFim datetime2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(1) AS QuantidadeVendas,
        SUM(v.[ValorBruto]) AS TotalBruto,
        SUM(v.[ValorComissao]) AS TotalComissao,
        SUM(v.[ValorLiquido]) AS TotalLiquidoVendedores
    FROM [TBL_VENDA] v
    WHERE
        (@DataInicio IS NULL OR v.[DataCriacao] >= @DataInicio)
        AND (@DataFim IS NULL OR v.[DataCriacao] <= @DataFim);
END;
GO

/*
   usp_OmniUsuarioCartoes
   Lista os cartoes ativos de um usuario.
   Nao retorna o TokenGateway para evitar expor dado sensivel.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniUsuarioCartoes]
    @UsuarioId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Tipo],
        [Apelido],
        [Bandeira],
        [Ultimos4Digitos],
        [MesValidade],
        [AnoValidade],
        [Ativo],
        [IsPrincipal],
        [DataCadastro],
        [DataAtualizacao]
    FROM [TBL_USUARIO_CARTAO_PAGAMENTO]
    WHERE [UsuarioId] = @UsuarioId AND [Ativo] = 1
    ORDER BY [IsPrincipal] DESC, [Id];
END;
GO

/*
   usp_OmniUsuarioCartaoDesativar
   Desativa o cartao sem apagar do banco.
   Isso preserva historico e evita perda de referencia em pagamentos antigos.
*/
CREATE OR ALTER PROCEDURE [dbo].[usp_OmniUsuarioCartaoDesativar]
    @UsuarioId int,
    @CartaoId int
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [TBL_USUARIO_CARTAO_PAGAMENTO]
    SET
        [Ativo] = 0,
        [IsPrincipal] = 0,
        [DataAtualizacao] = SYSUTCDATETIME()
    WHERE [Id] = @CartaoId AND [UsuarioId] = @UsuarioId;

    IF @@ROWCOUNT = 0
        THROW 51008, 'Cartao nao encontrado para este usuario.', 1;
END;
GO

/* ============================================================
   TRIGGERS
   ============================================================ */

/*
   TR_OMNI_PRODUTOS_BloquearDelete
   Impede exclusao fisica de produto.
   Quando alguem tenta deletar, o banco transforma em StatusPublicacao = Desativado.
*/
CREATE OR ALTER TRIGGER [dbo].[TR_OMNI_PRODUTOS_BloquearDelete]
ON [dbo].[TBL_PRODUTOS]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET
        p.[StatusPublicacao] = N'Desativado',
        p.[DtAtualizacao] = SYSDATETIMEOFFSET()
    FROM [TBL_PRODUTOS] p
    INNER JOIN deleted d ON d.[Id] = p.[Id];
END;
GO

/*
   TR_OMNI_PRODUTOS_BloquearCamposIdentidade
   Impede alteracao de Nome, Categoria e SKU.
   Se o produto foi cadastrado errado, a regra e desativar e criar outro.
*/
CREATE OR ALTER TRIGGER [dbo].[TR_OMNI_PRODUTOS_BloquearCamposIdentidade]
ON [dbo].[TBL_PRODUTOS]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE([Nome]) OR UPDATE([Categoria]) OR UPDATE([Sku])
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM inserted i
            INNER JOIN deleted d ON d.[Id] = i.[Id]
            WHERE
                ISNULL(i.[Nome], '') <> ISNULL(d.[Nome], '')
                OR ISNULL(i.[Categoria], '') <> ISNULL(d.[Categoria], '')
                OR ISNULL(i.[Sku], '') <> ISNULL(d.[Sku], '')
        )
        BEGIN
            THROW 51006, 'Nome, categoria e SKU nao podem ser alterados. Desative o produto e cadastre um novo.', 1;
        END;
    END;
END;
GO

/*
   TR_OMNI_VENDA_ValidarComissao
   Protege a regra financeira:
   comissao nao pode ser maior que o valor bruto e o liquido deve bater.
*/
CREATE OR ALTER TRIGGER [dbo].[TR_OMNI_VENDA_ValidarComissao]
ON [dbo].[TBL_VENDA]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted
        WHERE
            [ValorComissao] > [ValorBruto]
            OR [ValorLiquido] <> ROUND([ValorBruto] - [ValorComissao], 2)
    )
    BEGIN
        THROW 51007, 'Valores de comissao invalidos para a venda.', 1;
    END;
END;
GO

/*
   TR_OMNI_REPASSE_DataEfetiva
   Quando o repasse muda para Pago, preenche automaticamente a data efetiva.
*/
CREATE OR ALTER TRIGGER [dbo].[TR_OMNI_REPASSE_DataEfetiva]
ON [dbo].[TBL_REPASSE_VENDEDOR]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1
        RETURN;

    UPDATE r
    SET r.[DataEfetivaRepasse] = COALESCE(r.[DataEfetivaRepasse], SYSUTCDATETIME())
    FROM [TBL_REPASSE_VENDEDOR] r
    INNER JOIN inserted i ON i.[Id] = r.[Id]
    LEFT JOIN deleted d ON d.[Id] = i.[Id]
    WHERE
        i.[StatusRepasse] = N'Pago'
        AND ISNULL(d.[StatusRepasse], '') <> N'Pago'
        AND r.[DataEfetivaRepasse] IS NULL;
END;
GO

/* ============================================================
   CONSULTAS UTEIS PARA VISUALIZAR OS CENARIOS
   ============================================================ */

-- 1) Visualizar indices clustered e nonclustered criados no banco.
SELECT
    t.[name] AS Tabela,
    i.[name] AS Indice,
    i.[type_desc] AS TipoIndice,
    i.[is_unique] AS EhUnico,
    i.[is_primary_key] AS EhChavePrimaria
FROM sys.indexes i
INNER JOIN sys.tables t ON t.[object_id] = i.[object_id]
WHERE t.[is_ms_shipped] = 0
ORDER BY t.[name], i.[type_desc], i.[name];
GO

-- 2) Visualizar produtos publicos que aparecem no catalogo.
EXEC [dbo].[usp_OmniProdutoCatalogo]
    @Nome = NULL,
    @Categoria = NULL,
    @MinPreco = NULL,
    @MaxPreco = NULL,
    @Page = 1,
    @PageSize = 10;
GO

-- 3) Visualizar total de comissao da plataforma.
EXEC [dbo].[usp_OmniResumoComissoes]
    @DataInicio = NULL,
    @DataFim = NULL;
GO
