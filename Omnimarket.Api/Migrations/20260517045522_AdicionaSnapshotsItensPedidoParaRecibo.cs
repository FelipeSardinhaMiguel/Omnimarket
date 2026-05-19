using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaSnapshotsItensPedidoParaRecibo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentoLojaSnapshot",
                table: "TBL_ITENS_PEDIDO",
                type: "varchar(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeLojaSnapshot",
                table: "TBL_ITENS_PEDIDO",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeProdutoSnapshot",
                table: "TBL_ITENS_PEDIDO",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoDocumentoLojaSnapshot",
                table: "TBL_ITENS_PEDIDO",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentoLojaSnapshot",
                table: "TBL_ITENS_PEDIDO");

            migrationBuilder.DropColumn(
                name: "NomeLojaSnapshot",
                table: "TBL_ITENS_PEDIDO");

            migrationBuilder.DropColumn(
                name: "NomeProdutoSnapshot",
                table: "TBL_ITENS_PEDIDO");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoLojaSnapshot",
                table: "TBL_ITENS_PEDIDO");
        }
    }
}
