using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SuporteCpfCnpjLojaEReciboPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE TBL_LOJA
                SET TipoDocumentoFiscal = 'CPF'
                WHERE TipoDocumentoFiscal = 'Cpf';
                """);

            migrationBuilder.Sql("""
                UPDATE TBL_LOJA
                SET TipoDocumentoFiscal = 'CNPJ'
                WHERE TipoDocumentoFiscal = 'Cnpj';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE TBL_LOJA
                SET TipoDocumentoFiscal = 'Cpf'
                WHERE TipoDocumentoFiscal = 'CPF';
                """);

            migrationBuilder.Sql("""
                UPDATE TBL_LOJA
                SET TipoDocumentoFiscal = 'Cnpj'
                WHERE TipoDocumentoFiscal = 'CNPJ';
                """);
        }
    }
}
