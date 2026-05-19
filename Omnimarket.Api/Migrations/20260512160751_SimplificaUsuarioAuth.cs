using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaUsuarioAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_ACEITE_USUARIO_TERMO");

            migrationBuilder.DropTable(
                name: "TBL_TERMO_POLITICA");

            migrationBuilder.DropIndex(
                name: "IX_TBL_USUARIO_RefreshTokenHash",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "DataConfirmacaoEmail",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "EmailConfirmacaoEnviadoEm",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "EmailConfirmacaoTokenExpiraEm",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "EmailConfirmacaoTokenHash",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiraEm",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "RefreshTokenRevogadoEm",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "ResetSenhaSolicitadoEm",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "ResetSenhaTokenExpiraEm",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "ResetSenhaTokenHash",
                table: "TBL_USUARIO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataConfirmacaoEmail",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmacaoEnviadoEm",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmacaoTokenExpiraEm",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmacaoTokenHash",
                table: "TBL_USUARIO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "TBL_USUARIO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiraEm",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenHash",
                table: "TBL_USUARIO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenRevogadoEm",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetSenhaSolicitadoEm",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetSenhaTokenExpiraEm",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetSenhaTokenHash",
                table: "TBL_USUARIO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_TERMO_POLITICA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataPublicacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resumo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Texto = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Versao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_TERMO_POLITICA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ACEITE_USUARIO_TERMO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermoPoliticaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DataAceite = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentificadorAceite = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ResumoAceito = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    VersaoTermo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USUARIO_RefreshTokenHash",
                table: "TBL_USUARIO",
                column: "RefreshTokenHash",
                unique: true,
                filter: "[RefreshTokenHash] IS NOT NULL");

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
        }
    }
}
