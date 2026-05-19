using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaServicesFinanceiroFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [TBL_PRODUTOS_MIDIA]
                SET [Tipo] = 1
                WHERE [Tipo] = 2;
                """);

            migrationBuilder.DropTable(
                name: "TBL_DOCUMENTO_FISCAL_VENDA");

            migrationBuilder.DropTable(
                name: "TBL_HISTORICO_FINANCEIRO");

            migrationBuilder.DropTable(
                name: "TBL_RECIBO_FINANCEIRO");

            migrationBuilder.DropTable(
                name: "TBL_VENDA_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_REPASSE_VENDEDOR");

            migrationBuilder.DropColumn(
                name: "StatusFinanceiro",
                table: "TBL_VENDA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusFinanceiro",
                table: "TBL_VENDA",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TBL_DOCUMENTO_FISCAL_VENDA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LojaId = table.Column<int>(type: "int", nullable: true),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    ChaveAcesso = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroDocumento = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ProtocoloAutorizacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Serie = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlDanfe = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    UrlXml = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_DOCUMENTO_FISCAL_VENDA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_LOJA_LojaId",
                        column: x => x.LojaId,
                        principalTable: "TBL_LOJA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_USUARIO_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_VENDA_VendaId",
                        column: x => x.VendaId,
                        principalTable: "TBL_VENDA",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_HISTORICO_FINANCEIRO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: true),
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: true),
                    UsuarioResponsavelId = table.Column<int>(type: "int", nullable: true),
                    VendaId = table.Column<int>(type: "int", nullable: true),
                    DataMovimentacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    TipoMovimentacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataEfetivaRepasse = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPrevistaRepasse = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GatewayPayoutId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    StatusRepasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorRepasse = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: false),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    DataConfirmacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    StatusFinanceiro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorComissaoConsiderado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorLiquidoConsiderado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorPagoConsiderado = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: true),
                    RepasseVendedorId = table.Column<int>(type: "int", nullable: true),
                    VendaId = table.Column<int>(type: "int", nullable: true),
                    DataGeracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MensagemLegal = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    NumeroRecibo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    TextoAceiteVendedor = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ValorBruto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorComissao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorLiquido = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_ChaveAcesso",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "ChaveAcesso",
                unique: true,
                filter: "[ChaveAcesso] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_LojaId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_PedidoId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_VendaId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "VendaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_VendedorId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "VendedorId");

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
                name: "IX_TBL_VENDA_PAGAMENTO_PlanoPagamentoId",
                table: "TBL_VENDA_PAGAMENTO",
                column: "PlanoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_VENDA_PAGAMENTO_VendaId",
                table: "TBL_VENDA_PAGAMENTO",
                column: "VendaId");
        }
    }
}
