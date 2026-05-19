using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase13ComplianceFiscalVendas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_DOCUMENTO_FISCAL_VENDA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    LojaId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroDocumento = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Serie = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ChaveAcesso = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ProtocoloAutorizacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    UrlDanfe = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    UrlXml = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Observacao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_DOCUMENTO_FISCAL_VENDA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_LOJA_LojaId",
                        column: x => x.LojaId,
                        principalTable: "TBL_LOJA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_PEDIDO_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBL_PEDIDO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_USUARIO_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_DOCUMENTO_FISCAL_VENDA_TBL_VENDA_VendaId",
                        column: x => x.VendaId,
                        principalTable: "TBL_VENDA",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_ChaveAcesso",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "ChaveAcesso",
                unique: true,
                filter: "[ChaveAcesso] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_LojaId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "LojaId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_PedidoId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_VendaId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "VendaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_DOCUMENTO_FISCAL_VENDA_VendedorId",
                table: "TBL_DOCUMENTO_FISCAL_VENDA",
                column: "VendedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_DOCUMENTO_FISCAL_VENDA");
        }
    }
}
