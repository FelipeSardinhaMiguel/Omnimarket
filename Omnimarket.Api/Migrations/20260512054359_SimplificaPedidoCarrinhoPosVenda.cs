using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaPedidoCarrinhoPosVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_SOLICITACAO_POS_VENDA_INTERACAO");

            migrationBuilder.DropTable(
                name: "TBL_SOLICITACAO_POS_VENDA");

            migrationBuilder.DropTable(
                name: "TBL_PEDIDO_ENTREGA_LOJA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_PEDIDO_ENTREGA_LOJA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LojaId = table.Column<int>(type: "int", nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    CoberturaResumo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CondicoesAceitas = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataAceiteComprador = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAceiteVendedor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacaoStatus = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataRecusaVendedor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LojaEntregaOpcaoId = table.Column<int>(type: "int", nullable: true),
                    MotivoCancelamento = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    NomeOpcao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ObservacaoComprador = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ObservacaoVendedor = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    PrazoEntregaDias = table.Column<int>(type: "int", nullable: false),
                    StatusOperacional = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoEntregaId = table.Column<int>(type: "int", nullable: false),
                    ValorFrete = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PEDIDO_ENTREGA_LOJA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PEDIDO_ENTREGA_LOJA_TBL_LOJA_LojaId",
                        column: x => x.LojaId,
                        principalTable: "TBL_LOJA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_PEDIDO_ENTREGA_LOJA_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_SOLICITACAO_POS_VENDA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompradorId = table.Column<int>(type: "int", nullable: false),
                    LojaId = table.Column<int>(type: "int", nullable: false),
                    PedidoEntregaLojaId = table.Column<int>(type: "int", nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(1200)", maxLength: 1200, nullable: false),
                    Motivo = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SOLICITACAO_POS_VENDA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_TBL_LOJA_LojaId",
                        column: x => x.LojaId,
                        principalTable: "TBL_LOJA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_TBL_PEDIDO_ENTREGA_LOJA_PedidoEntregaLojaId",
                        column: x => x.PedidoEntregaLojaId,
                        principalTable: "TBL_PEDIDO_ENTREGA_LOJA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_TBL_USUARIO_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_TBL_USUARIO_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_TBL_VENDA_VendaId",
                        column: x => x.VendaId,
                        principalTable: "TBL_VENDA",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_SOLICITACAO_POS_VENDA_INTERACAO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AutorUsuarioId = table.Column<int>(type: "int", nullable: true),
                    SolicitacaoPosVendaId = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mensagem = table.Column<string>(type: "varchar(1200)", maxLength: 1200, nullable: false),
                    Origem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusResultante = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SOLICITACAO_POS_VENDA_INTERACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_INTERACAO_TBL_SOLICITACAO_POS_VENDA_SolicitacaoPosVendaId",
                        column: x => x.SolicitacaoPosVendaId,
                        principalTable: "TBL_SOLICITACAO_POS_VENDA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_SOLICITACAO_POS_VENDA_INTERACAO_TBL_USUARIO_AutorUsuarioId",
                        column: x => x.AutorUsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PEDIDO_ENTREGA_LOJA_LojaId",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PEDIDO_ENTREGA_LOJA_PedidoId",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_Codigo",
                table: "TBL_SOLICITACAO_POS_VENDA",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_CompradorId",
                table: "TBL_SOLICITACAO_POS_VENDA",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_LojaId",
                table: "TBL_SOLICITACAO_POS_VENDA",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_PedidoEntregaLojaId",
                table: "TBL_SOLICITACAO_POS_VENDA",
                column: "PedidoEntregaLojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_PedidoId",
                table: "TBL_SOLICITACAO_POS_VENDA",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_VendaId",
                table: "TBL_SOLICITACAO_POS_VENDA",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_VendedorId",
                table: "TBL_SOLICITACAO_POS_VENDA",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_INTERACAO_AutorUsuarioId",
                table: "TBL_SOLICITACAO_POS_VENDA_INTERACAO",
                column: "AutorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SOLICITACAO_POS_VENDA_INTERACAO_SolicitacaoPosVendaId",
                table: "TBL_SOLICITACAO_POS_VENDA_INTERACAO",
                column: "SolicitacaoPosVendaId");
        }
    }
}
