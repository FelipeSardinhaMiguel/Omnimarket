using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase2Lojas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_LOJA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    NomeFantasia = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    EmailContato = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    TelefoneContato = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Cidade = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Uf = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    DtCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DtAtualizacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_LOJA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_LOJA_TBL_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LOJA_Slug",
                table: "TBL_LOJA",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LOJA_UsuarioId",
                table: "TBL_LOJA",
                column: "UsuarioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_LOJA");
        }
    }
}
