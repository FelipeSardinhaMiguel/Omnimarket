using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class RefatoraLojaEnderecoTelefone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "TBL_LOJA");

            migrationBuilder.DropColumn(
                name: "TelefoneContato",
                table: "TBL_LOJA");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "TBL_LOJA");

            migrationBuilder.AddColumn<int>(
                name: "EnderecoId",
                table: "TBL_LOJA",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TelefoneId",
                table: "TBL_LOJA",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LOJA_EnderecoId",
                table: "TBL_LOJA",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LOJA_TelefoneId",
                table: "TBL_LOJA",
                column: "TelefoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_LOJA_TBL_ENDERECO_EnderecoId",
                table: "TBL_LOJA",
                column: "EnderecoId",
                principalTable: "TBL_ENDERECO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_LOJA_TBL_TELEFONE_TelefoneId",
                table: "TBL_LOJA",
                column: "TelefoneId",
                principalTable: "TBL_TELEFONE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_LOJA_TBL_ENDERECO_EnderecoId",
                table: "TBL_LOJA");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_LOJA_TBL_TELEFONE_TelefoneId",
                table: "TBL_LOJA");

            migrationBuilder.DropIndex(
                name: "IX_TBL_LOJA_EnderecoId",
                table: "TBL_LOJA");

            migrationBuilder.DropIndex(
                name: "IX_TBL_LOJA_TelefoneId",
                table: "TBL_LOJA");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "TBL_LOJA");

            migrationBuilder.DropColumn(
                name: "TelefoneId",
                table: "TBL_LOJA");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "TBL_LOJA",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneContato",
                table: "TBL_LOJA",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uf",
                table: "TBL_LOJA",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
