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


CREATE TABLE [TBL_FORMA_PAGAMENTO] (
    [Id] int NOT NULL IDENTITY,
    [Nome] varchar(200) NOT NULL,
    [Ativo] bit NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    [Observacao] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_FORMA_PAGAMENTO] PRIMARY KEY ([Id])
);
GO


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


CREATE TABLE [TBL_CARRINHO] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_TBL_CARRINHO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_CARRINHO_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO


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


CREATE TABLE [TBL_TELEFONE] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [NumeroE164] varchar(200) NOT NULL,
    [IsPrincipal] bit NOT NULL,
    CONSTRAINT [PK_TBL_TELEFONE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_TELEFONE_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);
GO


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


CREATE UNIQUE INDEX [IX_TBL_ACEITE_USUARIO_TERMO_IdentificadorAceite] ON [TBL_ACEITE_USUARIO_TERMO] ([IdentificadorAceite]);
GO


CREATE INDEX [IX_TBL_ACEITE_USUARIO_TERMO_TermoPoliticaId] ON [TBL_ACEITE_USUARIO_TERMO] ([TermoPoliticaId]);
GO


CREATE INDEX [IX_TBL_ACEITE_USUARIO_TERMO_UsuarioId] ON [TBL_ACEITE_USUARIO_TERMO] ([UsuarioId]);
GO


CREATE UNIQUE INDEX [IX_TBL_CARRINHO_UsuarioId] ON [TBL_CARRINHO] ([UsuarioId]);
GO


CREATE INDEX [IX_TBL_ENDERECO_UsuarioId] ON [TBL_ENDERECO] ([UsuarioId]);
GO


CREATE UNIQUE INDEX [IX_TBL_FORMA_PAGAMENTO_Nome] ON [TBL_FORMA_PAGAMENTO] ([Nome]);
GO


CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_PedidoId] ON [TBL_HISTORICO_FINANCEIRO] ([PedidoId]);
GO


CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_PlanoPagamentoId] ON [TBL_HISTORICO_FINANCEIRO] ([PlanoPagamentoId]);
GO


CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_UsuarioResponsavelId] ON [TBL_HISTORICO_FINANCEIRO] ([UsuarioResponsavelId]);
GO


CREATE INDEX [IX_TBL_HISTORICO_FINANCEIRO_VendaId] ON [TBL_HISTORICO_FINANCEIRO] ([VendaId]);
GO


CREATE INDEX [IX_TBL_ITEM_CARRINHO_CarrinhoId] ON [TBL_ITEM_CARRINHO] ([CarrinhoId]);
GO


CREATE INDEX [IX_TBL_ITEM_CARRINHO_ProdutoId] ON [TBL_ITEM_CARRINHO] ([ProdutoId]);
GO


CREATE INDEX [IX_TBL_ITEM_PLANO_PAGAMENTO_CondicaoPagamentoId] ON [TBL_ITEM_PLANO_PAGAMENTO] ([CondicaoPagamentoId]);
GO


CREATE INDEX [IX_TBL_ITEM_PLANO_PAGAMENTO_FormaPagamentoId] ON [TBL_ITEM_PLANO_PAGAMENTO] ([FormaPagamentoId]);
GO


CREATE INDEX [IX_TBL_ITEM_PLANO_PAGAMENTO_PlanoPagamentoId] ON [TBL_ITEM_PLANO_PAGAMENTO] ([PlanoPagamentoId]);
GO


CREATE INDEX [IX_TBL_ITENS_PEDIDO_PedidoId] ON [TBL_ITENS_PEDIDO] ([PedidoId]);
GO


CREATE INDEX [IX_TBL_ITENS_PEDIDO_ProdutoId] ON [TBL_ITENS_PEDIDO] ([ProdutoId]);
GO


CREATE UNIQUE INDEX [IX_TBL_LOJA_Slug] ON [TBL_LOJA] ([Slug]);
GO


CREATE UNIQUE INDEX [IX_TBL_LOJA_UsuarioId] ON [TBL_LOJA] ([UsuarioId]);
GO


CREATE INDEX [IX_TBL_PEDIDO_UsuarioId] ON [TBL_PEDIDO] ([UsuarioId]);
GO


CREATE UNIQUE INDEX [IX_TBL_PLANO_PAGAMENTO_PedidoId] ON [TBL_PLANO_PAGAMENTO] ([PedidoId]);
GO


CREATE UNIQUE INDEX [IX_TBL_PRODUTOS_Sku] ON [TBL_PRODUTOS] ([Sku]);
GO


CREATE INDEX [IX_TBL_PRODUTOS_UsuarioId] ON [TBL_PRODUTOS] ([UsuarioId]);
GO


CREATE INDEX [IX_TBL_PRODUTOS_MIDIA_ProdutoId] ON [TBL_PRODUTOS_MIDIA] ([ProdutoId]);
GO


CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_PedidoId] ON [TBL_RECIBO_FINANCEIRO] ([PedidoId]);
GO


CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_PlanoPagamentoId] ON [TBL_RECIBO_FINANCEIRO] ([PlanoPagamentoId]);
GO


CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_RepasseVendedorId] ON [TBL_RECIBO_FINANCEIRO] ([RepasseVendedorId]);
GO


CREATE INDEX [IX_TBL_RECIBO_FINANCEIRO_VendaId] ON [TBL_RECIBO_FINANCEIRO] ([VendaId]);
GO


CREATE INDEX [IX_TBL_REPASSE_VENDEDOR_VendaId] ON [TBL_REPASSE_VENDEDOR] ([VendaId]);
GO


CREATE INDEX [IX_TBL_REPASSE_VENDEDOR_VendedorId] ON [TBL_REPASSE_VENDEDOR] ([VendedorId]);
GO


CREATE INDEX [IX_TBL_TELEFONE_UsuarioId] ON [TBL_TELEFONE] ([UsuarioId]);
GO


CREATE UNIQUE INDEX [IX_TBL_USUARIO_Cpf] ON [TBL_USUARIO] ([Cpf]);
GO


CREATE UNIQUE INDEX [IX_TBL_USUARIO_Email] ON [TBL_USUARIO] ([Email]);
GO


CREATE UNIQUE INDEX [IX_TBL_USUARIO_CARTAO_PAGAMENTO_TokenGateway] ON [TBL_USUARIO_CARTAO_PAGAMENTO] ([TokenGateway]);
GO


CREATE UNIQUE INDEX [IX_TBL_USUARIO_CARTAO_PAGAMENTO_UsuarioId_IsPrincipal] ON [TBL_USUARIO_CARTAO_PAGAMENTO] ([UsuarioId], [IsPrincipal]) WHERE [IsPrincipal] = 1;
GO


CREATE INDEX [IX_TBL_VENDA_PedidoId] ON [TBL_VENDA] ([PedidoId]);
GO


CREATE INDEX [IX_TBL_VENDA_VendedorId] ON [TBL_VENDA] ([VendedorId]);
GO


CREATE INDEX [IX_TBL_VENDA_PAGAMENTO_PlanoPagamentoId] ON [TBL_VENDA_PAGAMENTO] ([PlanoPagamentoId]);
GO


CREATE INDEX [IX_TBL_VENDA_PAGAMENTO_VendaId] ON [TBL_VENDA_PAGAMENTO] ([VendaId]);
GO


