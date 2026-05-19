using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase1Marketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comentarios",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "Disponivel",
                table: "TBL_PRODUTOS");

            migrationBuilder.RenameColumn(
                name: "QtdProdutos",
                table: "TBL_PRODUTOS",
                newName: "TotalAvaliacoes");

            migrationBuilder.RenameColumn(
                name: "Avaliacao",
                table: "TBL_PRODUTOS",
                newName: "Estoque");

            migrationBuilder.RenameColumn(
                name: "ValorUnitario",
                table: "TBL_ITENS_PEDIDO",
                newName: "ValorTotal");

            migrationBuilder.RenameColumn(
                name: "ValorSubtotal",
                table: "TBL_ITENS_PEDIDO",
                newName: "PrecoUnitario");

            migrationBuilder.RenameColumn(
                name: "QtdItens",
                table: "TBL_ITENS_PEDIDO",
                newName: "Quantidade");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "TBL_USUARIO",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "TBL_USUARIO",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AceitouTermos",
                table: "TBL_USUARIO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAceiteTermos",
                table: "TBL_USUARIO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "TBL_USUARIO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "TBL_PRODUTOS",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DtAtualizacao",
                table: "TBL_PRODUTOS",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "TBL_PRODUTOS",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusPublicacao",
                table: "TBL_PRODUTOS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CepEntrega",
                table: "TBL_PEDIDO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CidadeEntrega",
                table: "TBL_PEDIDO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ComplementoEntrega",
                table: "TBL_PEDIDO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeEnderecoEntrega",
                table: "TBL_PEDIDO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroEntrega",
                table: "TBL_PEDIDO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoLogradouroEntrega",
                table: "TBL_PEDIDO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UfEntrega",
                table: "TBL_PEDIDO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TipoLogradouro",
                table: "TBL_ENDERECO",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "TBL_CARRINHO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CARRINHO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_CARRINHO_TBL_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ITEM_CARRINHO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarrinhoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ITEM_CARRINHO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_ITEM_CARRINHO_TBL_CARRINHO_CarrinhoId",
                        column: x => x.CarrinhoId,
                        principalTable: "TBL_CARRINHO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_ITEM_CARRINHO_TBL_PRODUTOS_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "TBL_PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_Sku",
                table: "TBL_PRODUTOS",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_UsuarioId",
                table: "TBL_PRODUTOS",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PEDIDO_UsuarioId",
                table: "TBL_PEDIDO",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ITENS_PEDIDO_ProdutoId",
                table: "TBL_ITENS_PEDIDO",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CARRINHO_UsuarioId",
                table: "TBL_CARRINHO",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ITEM_CARRINHO_CarrinhoId",
                table: "TBL_ITEM_CARRINHO",
                column: "CarrinhoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ITEM_CARRINHO_ProdutoId",
                table: "TBL_ITEM_CARRINHO",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_ITENS_PEDIDO_TBL_PRODUTOS_ProdutoId",
                table: "TBL_ITENS_PEDIDO",
                column: "ProdutoId",
                principalTable: "TBL_PRODUTOS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PEDIDO_TBL_USUARIO_UsuarioId",
                table: "TBL_PEDIDO",
                column: "UsuarioId",
                principalTable: "TBL_USUARIO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_USUARIO_UsuarioId",
                table: "TBL_PRODUTOS",
                column: "UsuarioId",
                principalTable: "TBL_USUARIO",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_ITENS_PEDIDO_TBL_PRODUTOS_ProdutoId",
                table: "TBL_ITENS_PEDIDO");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PEDIDO_TBL_USUARIO_UsuarioId",
                table: "TBL_PEDIDO");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_USUARIO_UsuarioId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropTable(
                name: "TBL_ITEM_CARRINHO");

            migrationBuilder.DropTable(
                name: "TBL_CARRINHO");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PRODUTOS_Sku",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PRODUTOS_UsuarioId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PEDIDO_UsuarioId",
                table: "TBL_PEDIDO");

            migrationBuilder.DropIndex(
                name: "IX_TBL_ITENS_PEDIDO_ProdutoId",
                table: "TBL_ITENS_PEDIDO");

            migrationBuilder.DropColumn(
                name: "AceitouTermos",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "DataAceiteTermos",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "TBL_USUARIO");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "DtAtualizacao",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "StatusPublicacao",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "CepEntrega",
                table: "TBL_PEDIDO");

            migrationBuilder.DropColumn(
                name: "CidadeEntrega",
                table: "TBL_PEDIDO");

            migrationBuilder.DropColumn(
                name: "ComplementoEntrega",
                table: "TBL_PEDIDO");

            migrationBuilder.DropColumn(
                name: "NomeEnderecoEntrega",
                table: "TBL_PEDIDO");

            migrationBuilder.DropColumn(
                name: "NumeroEntrega",
                table: "TBL_PEDIDO");

            migrationBuilder.DropColumn(
                name: "TipoLogradouroEntrega",
                table: "TBL_PEDIDO");

            migrationBuilder.DropColumn(
                name: "UfEntrega",
                table: "TBL_PEDIDO");

            migrationBuilder.RenameColumn(
                name: "TotalAvaliacoes",
                table: "TBL_PRODUTOS",
                newName: "QtdProdutos");

            migrationBuilder.RenameColumn(
                name: "Estoque",
                table: "TBL_PRODUTOS",
                newName: "Avaliacao");

            migrationBuilder.RenameColumn(
                name: "ValorTotal",
                table: "TBL_ITENS_PEDIDO",
                newName: "ValorUnitario");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "TBL_ITENS_PEDIDO",
                newName: "QtdItens");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "TBL_ITENS_PEDIDO",
                newName: "ValorSubtotal");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "TBL_USUARIO",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "TBL_USUARIO",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AddColumn<string>(
                name: "Comentarios",
                table: "TBL_PRODUTOS",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Disponivel",
                table: "TBL_PRODUTOS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TipoLogradouro",
                table: "TBL_ENDERECO",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
