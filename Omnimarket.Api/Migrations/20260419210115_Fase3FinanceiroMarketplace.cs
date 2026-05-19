using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase3FinanceiroMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_CONDICAO_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    QuantidadeParcelas = table.Column<int>(type: "int", nullable: false),
                    IntervaloDias = table.Column<int>(type: "int", nullable: false),
                    PossuiJuros = table.Column<bool>(type: "bit", nullable: false),
                    PercentualJuros = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorEntrada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CONDICAO_PAGAMENTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_FORMA_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_FORMA_PAGAMENTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PLANO_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataConfirmacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacoes = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PLANO_PAGAMENTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PLANO_PAGAMENTO_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_TERMO_POLITICA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Versao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Resumo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Texto = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataPublicacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_TERMO_POLITICA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_VENDA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    ValorBruto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorComissao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorLiquido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorFrete = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorFixoComissaoAplicado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentualComissaoAplicado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FormulaComissaoAplicada = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    StatusVenda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusFinanceiro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_VENDA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_VENDA_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_VENDA_TBL_USUARIO_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_ITEM_PLANO_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: false),
                    FormaPagamentoId = table.Column<int>(type: "int", nullable: false),
                    CondicaoPagamentoId = table.Column<int>(type: "int", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantidadeParcelas = table.Column<int>(type: "int", nullable: false),
                    ValorParcela = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoAutorizacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ITEM_PLANO_PAGAMENTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_ITEM_PLANO_PAGAMENTO_TBL_CONDICAO_PAGAMENTO_CondicaoPagamentoId",
                        column: x => x.CondicaoPagamentoId,
                        principalTable: "TBL_CONDICAO_PAGAMENTO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_ITEM_PLANO_PAGAMENTO_TBL_FORMA_PAGAMENTO_FormaPagamentoId",
                        column: x => x.FormaPagamentoId,
                        principalTable: "TBL_FORMA_PAGAMENTO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_ITEM_PLANO_PAGAMENTO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId",
                        column: x => x.PlanoPagamentoId,
                        principalTable: "TBL_PLANO_PAGAMENTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ACEITE_USUARIO_TERMO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TermoPoliticaId = table.Column<int>(type: "int", nullable: false),
                    VersaoTermo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ResumoAceito = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IdentificadorAceite = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataAceite = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ACEITE_USUARIO_TERMO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_ACEITE_USUARIO_TERMO_TBL_TERMO_POLITICA_TermoPoliticaId",
                        column: x => x.TermoPoliticaId,
                        principalTable: "TBL_TERMO_POLITICA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_ACEITE_USUARIO_TERMO_TBL_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_HISTORICO_FINANCEIRO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: true),
                    VendaId = table.Column<int>(type: "int", nullable: true),
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: true),
                    TipoMovimentacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataMovimentacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioResponsavelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_HISTORICO_FINANCEIRO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_HISTORICO_FINANCEIRO_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_HISTORICO_FINANCEIRO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId",
                        column: x => x.PlanoPagamentoId,
                        principalTable: "TBL_PLANO_PAGAMENTO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_HISTORICO_FINANCEIRO_TBL_USUARIO_UsuarioResponsavelId",
                        column: x => x.UsuarioResponsavelId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_HISTORICO_FINANCEIRO_TBL_VENDA_VendaId",
                        column: x => x.VendaId,
                        principalTable: "TBL_VENDA",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_REPASSE_VENDEDOR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    ValorRepasse = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataPrevistaRepasse = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataEfetivaRepasse = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusRepasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GatewayPayoutId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_REPASSE_VENDEDOR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_REPASSE_VENDEDOR_TBL_USUARIO_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_REPASSE_VENDEDOR_TBL_VENDA_VendaId",
                        column: x => x.VendaId,
                        principalTable: "TBL_VENDA",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_VENDA_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: false),
                    ValorPagoConsiderado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorComissaoConsiderado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorLiquidoConsiderado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusFinanceiro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataConfirmacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_VENDA_PAGAMENTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_VENDA_PAGAMENTO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId",
                        column: x => x.PlanoPagamentoId,
                        principalTable: "TBL_PLANO_PAGAMENTO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_VENDA_PAGAMENTO_TBL_VENDA_VendaId",
                        column: x => x.VendaId,
                        principalTable: "TBL_VENDA",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_RECIBO_FINANCEIRO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    VendaId = table.Column<int>(type: "int", nullable: true),
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: true),
                    RepasseVendedorId = table.Column<int>(type: "int", nullable: true),
                    NumeroRecibo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    MensagemLegal = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    TextoAceiteVendedor = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ValorBruto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorComissao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorLiquido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataGeracao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RECIBO_FINANCEIRO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RECIBO_FINANCEIRO_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RECIBO_FINANCEIRO_TBL_PLANO_PAGAMENTO_PlanoPagamentoId",
                        column: x => x.PlanoPagamentoId,
                        principalTable: "TBL_PLANO_PAGAMENTO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RECIBO_FINANCEIRO_TBL_REPASSE_VENDEDOR_RepasseVendedorId",
                        column: x => x.RepasseVendedorId,
                        principalTable: "TBL_REPASSE_VENDEDOR",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RECIBO_FINANCEIRO_TBL_VENDA_VendaId",
                        column: x => x.VendaId,
                        principalTable: "TBL_VENDA",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "TBL_CONDICAO_PAGAMENTO",
                columns: new[] { "Id", "Ativa", "DataCriacao", "IntervaloDias", "Nome", "Observacao", "PercentualJuros", "PossuiJuros", "QuantidadeParcelas", "ValorEntrada" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "A vista", "Pagamento integral em uma parcela.", 0m, false, 1, 0m },
                    { 2, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, "2x sem juros", "Parcelamento em 2 vezes sem juros.", 0m, false, 2, 0m },
                    { 3, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, "3x sem juros", "Parcelamento em 3 vezes sem juros.", 0m, false, 3, 0m },
                    { 4, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, "Pix parcelado (futuro)", "Condicao prevista para evolucao futura.", 2.50m, true, 2, 0m }
                });

            migrationBuilder.InsertData(
                table: "TBL_FORMA_PAGAMENTO",
                columns: new[] { "Id", "Ativo", "DataCriacao", "Nome", "Observacao" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pix", "Pagamento instantaneo via Pix." },
                    { 2, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dinheiro", "Pagamento em dinheiro na retirada." },
                    { 3, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cartao de Debito", "Pagamento em cartao de debito." },
                    { 4, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cartao de Credito", "Pagamento em cartao de credito." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ACEITE_USUARIO_TERMO_IdentificadorAceite",
                table: "TBL_ACEITE_USUARIO_TERMO",
                column: "IdentificadorAceite",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ACEITE_USUARIO_TERMO_TermoPoliticaId",
                table: "TBL_ACEITE_USUARIO_TERMO",
                column: "TermoPoliticaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ACEITE_USUARIO_TERMO_UsuarioId",
                table: "TBL_ACEITE_USUARIO_TERMO",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_FORMA_PAGAMENTO_Nome",
                table: "TBL_FORMA_PAGAMENTO",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_HISTORICO_FINANCEIRO_PedidoId",
                table: "TBL_HISTORICO_FINANCEIRO",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_HISTORICO_FINANCEIRO_PlanoPagamentoId",
                table: "TBL_HISTORICO_FINANCEIRO",
                column: "PlanoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_HISTORICO_FINANCEIRO_UsuarioResponsavelId",
                table: "TBL_HISTORICO_FINANCEIRO",
                column: "UsuarioResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_HISTORICO_FINANCEIRO_VendaId",
                table: "TBL_HISTORICO_FINANCEIRO",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ITEM_PLANO_PAGAMENTO_CondicaoPagamentoId",
                table: "TBL_ITEM_PLANO_PAGAMENTO",
                column: "CondicaoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ITEM_PLANO_PAGAMENTO_FormaPagamentoId",
                table: "TBL_ITEM_PLANO_PAGAMENTO",
                column: "FormaPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ITEM_PLANO_PAGAMENTO_PlanoPagamentoId",
                table: "TBL_ITEM_PLANO_PAGAMENTO",
                column: "PlanoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PLANO_PAGAMENTO_PedidoId",
                table: "TBL_PLANO_PAGAMENTO",
                column: "PedidoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RECIBO_FINANCEIRO_PedidoId",
                table: "TBL_RECIBO_FINANCEIRO",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RECIBO_FINANCEIRO_PlanoPagamentoId",
                table: "TBL_RECIBO_FINANCEIRO",
                column: "PlanoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RECIBO_FINANCEIRO_RepasseVendedorId",
                table: "TBL_RECIBO_FINANCEIRO",
                column: "RepasseVendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RECIBO_FINANCEIRO_VendaId",
                table: "TBL_RECIBO_FINANCEIRO",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_REPASSE_VENDEDOR_VendaId",
                table: "TBL_REPASSE_VENDEDOR",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_REPASSE_VENDEDOR_VendedorId",
                table: "TBL_REPASSE_VENDEDOR",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_VENDA_PedidoId",
                table: "TBL_VENDA",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_VENDA_VendedorId",
                table: "TBL_VENDA",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_VENDA_PAGAMENTO_PlanoPagamentoId",
                table: "TBL_VENDA_PAGAMENTO",
                column: "PlanoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_VENDA_PAGAMENTO_VendaId",
                table: "TBL_VENDA_PAGAMENTO",
                column: "VendaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_ACEITE_USUARIO_TERMO");

            migrationBuilder.DropTable(
                name: "TBL_HISTORICO_FINANCEIRO");

            migrationBuilder.DropTable(
                name: "TBL_ITEM_PLANO_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_RECIBO_FINANCEIRO");

            migrationBuilder.DropTable(
                name: "TBL_VENDA_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_TERMO_POLITICA");

            migrationBuilder.DropTable(
                name: "TBL_CONDICAO_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_FORMA_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_REPASSE_VENDEDOR");

            migrationBuilder.DropTable(
                name: "TBL_PLANO_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_VENDA");
        }
    }
}
