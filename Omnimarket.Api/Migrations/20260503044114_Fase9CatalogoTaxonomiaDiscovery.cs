using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase9CatalogoTaxonomiaDiscovery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaCatalogoId",
                table: "TBL_PRODUTOS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoPaiId",
                table: "TBL_PRODUTOS",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_CATEGORIA_CATALOGO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CategoriaPaiId = table.Column<int>(type: "int", nullable: true),
                    Caminho = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    DtCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CATEGORIA_CATALOGO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_CATEGORIA_CATALOGO_TBL_CATEGORIA_CATALOGO_CategoriaPaiId",
                        column: x => x.CategoriaPaiId,
                        principalTable: "TBL_CATEGORIA_CATALOGO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_CATEGORIA_ATRIBUTO_CATALOGO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Obrigatorio = table.Column<bool>(type: "bit", nullable: false),
                    Filtravel = table.Column<bool>(type: "bit", nullable: false),
                    ValoresPermitidos = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CATEGORIA_ATRIBUTO_CATALOGO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_CATEGORIA_ATRIBUTO_CATALOGO_TBL_CATEGORIA_CATALOGO_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "TBL_CATEGORIA_CATALOGO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PRODUTO_ATRIBUTO_VALOR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaAtributoId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ValorNormalizado = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PRODUTO_ATRIBUTO_VALOR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PRODUTO_ATRIBUTO_VALOR_TBL_CATEGORIA_ATRIBUTO_CATALOGO_CategoriaAtributoId",
                        column: x => x.CategoriaAtributoId,
                        principalTable: "TBL_CATEGORIA_ATRIBUTO_CATALOGO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_PRODUTO_ATRIBUTO_VALOR_TBL_PRODUTOS_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "TBL_PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_CategoriaCatalogoId",
                table: "TBL_PRODUTOS",
                column: "CategoriaCatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTOS_ProdutoPaiId",
                table: "TBL_PRODUTOS",
                column: "ProdutoPaiId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CATEGORIA_ATRIBUTO_CATALOGO_CategoriaId_Slug",
                table: "TBL_CATEGORIA_ATRIBUTO_CATALOGO",
                columns: new[] { "CategoriaId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CATEGORIA_CATALOGO_CategoriaPaiId",
                table: "TBL_CATEGORIA_CATALOGO",
                column: "CategoriaPaiId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CATEGORIA_CATALOGO_Slug",
                table: "TBL_CATEGORIA_CATALOGO",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTO_ATRIBUTO_VALOR_CategoriaAtributoId",
                table: "TBL_PRODUTO_ATRIBUTO_VALOR",
                column: "CategoriaAtributoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PRODUTO_ATRIBUTO_VALOR_ProdutoId_CategoriaAtributoId",
                table: "TBL_PRODUTO_ATRIBUTO_VALOR",
                columns: new[] { "ProdutoId", "CategoriaAtributoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_CATEGORIA_CATALOGO_CategoriaCatalogoId",
                table: "TBL_PRODUTOS",
                column: "CategoriaCatalogoId",
                principalTable: "TBL_CATEGORIA_CATALOGO",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_PRODUTOS_ProdutoPaiId",
                table: "TBL_PRODUTOS",
                column: "ProdutoPaiId",
                principalTable: "TBL_PRODUTOS",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_CATEGORIA_CATALOGO_CategoriaCatalogoId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_PRODUTOS_ProdutoPaiId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropTable(
                name: "TBL_PRODUTO_ATRIBUTO_VALOR");

            migrationBuilder.DropTable(
                name: "TBL_CATEGORIA_ATRIBUTO_CATALOGO");

            migrationBuilder.DropTable(
                name: "TBL_CATEGORIA_CATALOGO");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PRODUTOS_CategoriaCatalogoId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PRODUTOS_ProdutoPaiId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "CategoriaCatalogoId",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "ProdutoPaiId",
                table: "TBL_PRODUTOS");
        }
    }
}
