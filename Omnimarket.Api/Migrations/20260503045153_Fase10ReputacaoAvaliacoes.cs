using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase10ReputacaoAvaliacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MediaAvaliacao",
                table: "TBL_LOJA",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalAvaliacoes",
                table: "TBL_LOJA",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TBL_AVALIACAO_PRODUTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    LojaId = table.Column<int>(type: "int", nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    NotaProduto = table.Column<int>(type: "int", nullable: false),
                    NotaLoja = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Comentario = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CompraVerificada = table.Column<bool>(type: "bit", nullable: false),
                    RecomendaProduto = table.Column<bool>(type: "bit", nullable: false),
                    DtCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DtAtualizacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_AVALIACAO_PRODUTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_AVALIACAO_PRODUTO_TBL_LOJA_LojaId",
                        column: x => x.LojaId,
                        principalTable: "TBL_LOJA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_AVALIACAO_PRODUTO_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_AVALIACAO_PRODUTO_TBL_PRODUTOS_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "TBL_PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_AVALIACAO_PRODUTO_TBL_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_AVALIACAO_PRODUTO_LojaId",
                table: "TBL_AVALIACAO_PRODUTO",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_AVALIACAO_PRODUTO_PedidoId",
                table: "TBL_AVALIACAO_PRODUTO",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_AVALIACAO_PRODUTO_ProdutoId_PedidoId_UsuarioId",
                table: "TBL_AVALIACAO_PRODUTO",
                columns: new[] { "ProdutoId", "PedidoId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_AVALIACAO_PRODUTO_UsuarioId",
                table: "TBL_AVALIACAO_PRODUTO",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_AVALIACAO_PRODUTO");

            migrationBuilder.DropColumn(
                name: "MediaAvaliacao",
                table: "TBL_LOJA");

            migrationBuilder.DropColumn(
                name: "TotalAvaliacoes",
                table: "TBL_LOJA");
        }
    }
}
