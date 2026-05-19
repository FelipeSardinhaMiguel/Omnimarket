using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase11AuthRefreshRecuperacaoEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USUARIO_RefreshTokenHash",
                table: "TBL_USUARIO",
                column: "RefreshTokenHash",
                unique: true,
                filter: "[RefreshTokenHash] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
