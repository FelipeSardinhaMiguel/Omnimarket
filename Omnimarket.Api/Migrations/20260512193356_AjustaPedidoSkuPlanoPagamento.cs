using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AjustaPedidoSkuPlanoPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_PRODUTOS_LojaId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PRODUTOS_Sku",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "TBL_PLANO_PAGAMENTO");

            migrationBuilder.AddColumn<int>(
                name: "FormaPagamentoId",
                table: "TBL_PLANO_PAGAMENTO",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_LojaId_Sku",
                table: "TBL_PRODUTOS",
                columns: new[] { "LojaId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PLANO_PAGAMENTO_FormaPagamentoId",
                table: "TBL_PLANO_PAGAMENTO",
                column: "FormaPagamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PLANO_PAGAMENTO_TBL_FORMA_PAGAMENTO_FormaPagamentoId",
                table: "TBL_PLANO_PAGAMENTO",
                column: "FormaPagamentoId",
                principalTable: "TBL_FORMA_PAGAMENTO",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PLANO_PAGAMENTO_TBL_FORMA_PAGAMENTO_FormaPagamentoId",
                table: "TBL_PLANO_PAGAMENTO");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PRODUTOS_LojaId_Sku",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PLANO_PAGAMENTO_FormaPagamentoId",
                table: "TBL_PLANO_PAGAMENTO");

            migrationBuilder.DropColumn(
                name: "FormaPagamentoId",
                table: "TBL_PLANO_PAGAMENTO");

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "TBL_PLANO_PAGAMENTO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_LojaId",
                table: "TBL_PRODUTOS",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_Sku",
                table: "TBL_PRODUTOS",
                column: "Sku",
                unique: true);
        }
    }
}
