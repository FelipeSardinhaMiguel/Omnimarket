using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase7EntregaIntermediada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_LOJA_ENTREGA_OPCAO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LojaId = table.Column<int>(type: "int", nullable: false),
                    TipoEntregaId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ValorFrete = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrazoMinDias = table.Column<int>(type: "int", nullable: false),
                    PrazoMaxDias = table.Column<int>(type: "int", nullable: false),
                    CidadeCobertura = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    UfCobertura = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CepInicial = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CepFinal = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    RaioKm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_LOJA_ENTREGA_OPCAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_LOJA_ENTREGA_OPCAO_TBL_LOJA_LojaId",
                        column: x => x.LojaId,
                        principalTable: "TBL_LOJA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PEDIDO_ENTREGA_LOJA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    LojaId = table.Column<int>(type: "int", nullable: false),
                    LojaEntregaOpcaoId = table.Column<int>(type: "int", nullable: true),
                    TipoEntregaId = table.Column<int>(type: "int", nullable: false),
                    NomeOpcao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ValorFrete = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrazoMinDias = table.Column<int>(type: "int", nullable: false),
                    PrazoMaxDias = table.Column<int>(type: "int", nullable: false),
                    CoberturaResumo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CondicoesAceitas = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    StatusOperacional = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAceiteComprador = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacaoStatus = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoCancelamento = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ObservacaoComprador = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LOJA_ENTREGA_OPCAO_LojaId",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PEDIDO_ENTREGA_LOJA_LojaId",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PEDIDO_ENTREGA_LOJA_PedidoId",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                column: "PedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_LOJA_ENTREGA_OPCAO");

            migrationBuilder.DropTable(
                name: "TBL_PEDIDO_ENTREGA_LOJA");
        }
    }
}
