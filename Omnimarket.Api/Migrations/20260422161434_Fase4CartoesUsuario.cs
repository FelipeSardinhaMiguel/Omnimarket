using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase4CartoesUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_USUARIO_CARTAO_PAGAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apelido = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Bandeira = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Ultimos4Digitos = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    TokenGateway = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    MesValidade = table.Column<int>(type: "int", nullable: false),
                    AnoValidade = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.InsertData(
                table: "TBL_FORMA_PAGAMENTO",
                columns: new[] { "Id", "Ativo", "DataCriacao", "Nome", "Observacao" },
                values: new object[,]
                {
                    { 5, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vale Refeicao", "Pagamento com vale refeicao." },
                    { 6, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vale Alimentacao", "Pagamento com vale alimentacao." }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_USUARIO_CARTAO_PAGAMENTO");

            migrationBuilder.DeleteData(
                table: "TBL_FORMA_PAGAMENTO",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TBL_FORMA_PAGAMENTO",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
