using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase14AceiteRecusaSellerFulfillment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAceiteVendedor",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRecusaVendedor",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacaoVendedor",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAceiteVendedor",
                table: "TBL_PEDIDO_ENTREGA_LOJA");

            migrationBuilder.DropColumn(
                name: "DataRecusaVendedor",
                table: "TBL_PEDIDO_ENTREGA_LOJA");

            migrationBuilder.DropColumn(
                name: "ObservacaoVendedor",
                table: "TBL_PEDIDO_ENTREGA_LOJA");
        }
    }
}
