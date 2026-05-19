using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaDataContextFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_ITEM_PLANO_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_USUARIO_CARTAO_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "TBL_CONDICAO_PAGAMENTO");

            migrationBuilder.DeleteData(
                table: "TBL_FORMA_PAGAMENTO",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TBL_FORMA_PAGAMENTO",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "FormulaComissaoAplicada",
                table: "TBL_VENDA");

            migrationBuilder.DropColumn(
                name: "PercentualComissaoAplicado",
                table: "TBL_VENDA");

            migrationBuilder.DropColumn(
                name: "ValorComissao",
                table: "TBL_VENDA");

            migrationBuilder.DropColumn(
                name: "ValorFixoComissaoAplicado",
                table: "TBL_VENDA");

            migrationBuilder.DropColumn(
                name: "ValorFrete",
                table: "TBL_VENDA");

            migrationBuilder.DropColumn(
                name: "DataConfirmacao",
                table: "TBL_PLANO_PAGAMENTO");

            migrationBuilder.DropColumn(
                name: "GatewayTransactionId",
                table: "TBL_PLANO_PAGAMENTO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormulaComissaoAplicada",
                table: "TBL_VENDA",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PercentualComissaoAplicado",
                table: "TBL_VENDA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorComissao",
                table: "TBL_VENDA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFixoComissaoAplicado",
                table: "TBL_VENDA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFrete",
                table: "TBL_VENDA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataConfirmacao",
                table: "TBL_PLANO_PAGAMENTO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayTransactionId",
                table: "TBL_PLANO_PAGAMENTO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TBL_CONDICAO_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IntervaloDias = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    PercentualJuros = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PossuiJuros = table.Column<bool>(type: "bit", nullable: false),
                    QuantidadeParcelas = table.Column<int>(type: "int", nullable: false),
                    ValorEntrada = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CONDICAO_PAGAMENTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_USUARIO_CARTAO_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    AnoValidade = table.Column<int>(type: "int", nullable: false),
                    Apelido = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Bandeira = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    MesValidade = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenGateway = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Ultimos4Digitos = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_USUARIO_CARTAO_PAGAMENTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_USUARIO_CARTAO_PAGAMENTO_TBL_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ITEM_PLANO_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CondicaoPagamentoId = table.Column<int>(type: "int", nullable: true),
                    FormaPagamentoId = table.Column<int>(type: "int", nullable: false),
                    PlanoPagamentoId = table.Column<int>(type: "int", nullable: false),
                    CodigoAutorizacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    QuantidadeParcelas = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorParcela = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    { 5, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vale Refeicao", "Pagamento com vale refeicao." },
                    { 6, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vale Alimentacao", "Pagamento com vale alimentacao." }
                });

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
                name: "IX_TBL_USUARIO_CARTAO_PAGAMENTO_TokenGateway",
                table: "TBL_USUARIO_CARTAO_PAGAMENTO",
                column: "TokenGateway",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USUARIO_CARTAO_PAGAMENTO_UsuarioId_IsPrincipal",
                table: "TBL_USUARIO_CARTAO_PAGAMENTO",
                columns: new[] { "UsuarioId", "IsPrincipal" },
                unique: true,
                filter: "[IsPrincipal] = 1");
        }
    }
}
