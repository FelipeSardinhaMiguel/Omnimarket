using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaProdutoCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "EstoqueVersao",
                table: "TBL_PRODUTOS");

            migrationBuilder.DropColumn(
                name: "ProdutoPaiId",
                table: "TBL_PRODUTOS");

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('TBL_PRODUTOS', 'Disponivel') IS NOT NULL
                BEGIN
                    DECLARE @defaultConstraintDisponivel sysname;
                    SELECT @defaultConstraintDisponivel = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c]
                        ON [d].[parent_column_id] = [c].[column_id]
                       AND [d].[parent_object_id] = [c].[object_id]
                    WHERE [d].[parent_object_id] = OBJECT_ID(N'[TBL_PRODUTOS]')
                      AND [c].[name] = N'Disponivel';

                    IF @defaultConstraintDisponivel IS NOT NULL
                        EXEC(N'ALTER TABLE [TBL_PRODUTOS] DROP CONSTRAINT [' + @defaultConstraintDisponivel + '];');

                    ALTER TABLE [TBL_PRODUTOS] DROP COLUMN [Disponivel];
                END
                """);

            migrationBuilder.DropColumn(
                name: "PrazoMaxDias",
                table: "TBL_PEDIDO_ENTREGA_LOJA");

            migrationBuilder.DropColumn(
                name: "CepFinal",
                table: "TBL_LOJA_ENTREGA_OPCAO");

            migrationBuilder.DropColumn(
                name: "CepInicial",
                table: "TBL_LOJA_ENTREGA_OPCAO");

            migrationBuilder.DropColumn(
                name: "CidadeCobertura",
                table: "TBL_LOJA_ENTREGA_OPCAO");

            migrationBuilder.DropColumn(
                name: "PrazoMaxDias",
                table: "TBL_LOJA_ENTREGA_OPCAO");

            migrationBuilder.DropColumn(
                name: "RaioKm",
                table: "TBL_LOJA_ENTREGA_OPCAO");

            migrationBuilder.DropColumn(
                name: "UfCobertura",
                table: "TBL_LOJA_ENTREGA_OPCAO");

            migrationBuilder.RenameColumn(
                name: "PrazoMinDias",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                newName: "PrazoEntregaDias");

            migrationBuilder.RenameColumn(
                name: "PrazoMinDias",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                newName: "PrazoEntregaDias");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "TBL_PRODUTOS",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "TBL_PRODUTOS",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "TBL_PRODUTOS",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "TBL_PRODUTOS",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrazoEntregaDias",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                newName: "PrazoMinDias");

            migrationBuilder.RenameColumn(
                name: "PrazoEntregaDias",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                newName: "PrazoMinDias");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "TBL_PRODUTOS",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "TBL_PRODUTOS",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "TBL_PRODUTOS",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "TBL_PRODUTOS",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "CategoriaCatalogoId",
                table: "TBL_PRODUTOS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstoqueVersao",
                table: "TBL_PRODUTOS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoPaiId",
                table: "TBL_PRODUTOS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Disponivel",
                table: "TBL_PRODUTOS",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "PrazoMaxDias",
                table: "TBL_PEDIDO_ENTREGA_LOJA",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CepFinal",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CepInicial",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CidadeCobertura",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrazoMaxDias",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RaioKm",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UfCobertura",
                table: "TBL_LOJA_ENTREGA_OPCAO",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_CATEGORIA_CATALOGO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaPaiId = table.Column<int>(type: "int", nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    Caminho = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DtCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
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
                    Filtravel = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Obrigatorio = table.Column<bool>(type: "bit", nullable: false),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    CategoriaAtributoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
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
    }
}
