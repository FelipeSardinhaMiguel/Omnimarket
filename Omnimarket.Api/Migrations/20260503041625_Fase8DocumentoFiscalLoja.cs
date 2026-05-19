using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase8DocumentoFiscalLoja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentoFiscal",
                table: "TBL_LOJA",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoDocumentoFiscal",
                table: "TBL_LOJA",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LOJA_TipoDocumentoFiscal_DocumentoFiscal",
                table: "TBL_LOJA",
                columns: new[] { "TipoDocumentoFiscal", "DocumentoFiscal" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_LOJA_TipoDocumentoFiscal_DocumentoFiscal",
                table: "TBL_LOJA");

            migrationBuilder.DropColumn(
                name: "DocumentoFiscal",
                table: "TBL_LOJA");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoFiscal",
                table: "TBL_LOJA");
        }
    }
}
