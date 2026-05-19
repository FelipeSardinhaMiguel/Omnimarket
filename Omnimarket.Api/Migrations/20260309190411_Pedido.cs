using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Pedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_PEDIDO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoEntregaId = table.Column<int>(type: "int", nullable: false),
                    StatusPedidosId = table.Column<int>(type: "int", nullable: false),
                    ValorTotalProdutos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorFrete = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorTotalPedido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PEDIDO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PRODUTOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    QtdProdutos = table.Column<int>(type: "int", nullable: false),
                    MediaAvaliacao = table.Column<double>(type: "float", nullable: false),
                    Avaliacao = table.Column<int>(type: "int", nullable: false),
                    DtCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Comentarios = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PRODUTOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ITENS_PEDIDO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    QtdItens = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorSubtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ITENS_PEDIDO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_ITENS_PEDIDO_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PRODUTOS_MIDIA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ContentType = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    DtCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PRODUTOS_MIDIA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PRODUTOS_MIDIA_TBL_PRODUTOS_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "TBL_PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ITENS_PEDIDO_PedidoId",
                table: "TBL_ITENS_PEDIDO",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_MIDIA_ProdutoId",
                table: "TBL_PRODUTOS_MIDIA",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_ITENS_PEDIDO");

            migrationBuilder.DropTable(
                name: "TBL_PRODUTOS_MIDIA");

            migrationBuilder.DropTable(
                name: "TBL_PEDIDO");

            migrationBuilder.DropTable(
                name: "TBL_PRODUTOS");
        }
    }
}
