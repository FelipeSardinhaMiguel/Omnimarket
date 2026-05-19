using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class Fase6ProdutoVinculadoLoja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_USUARIO_UsuarioId",
                table: "TBL_PRODUTOS");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "TBL_PRODUTOS",
                newName: "LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_TBL_PRODUTOS_UsuarioId",
                table: "TBL_PRODUTOS",
                newName: "IX_TBL_PRODUTOS_LojaId");

            migrationBuilder.Sql(
                """
                INSERT INTO TBL_LOJA (UsuarioId, NomeFantasia, Slug, Descricao, EmailContato, TelefoneContato, Cidade, Uf, Ativa, DtCriacao, DtAtualizacao)
                SELECT
                    u.Id,
                    LEFT(CONCAT('Loja de ', u.Nome, ' ', u.Sobrenome), 120),
                    CONCAT('loja-migrada-usuario-', u.Id, '-', REPLACE(CONVERT(varchar(36), NEWID()), '-', '')),
                    'Loja criada automaticamente para manter o vinculo dos produtos existentes.',
                    NULL,
                    NULL,
                    NULL,
                    NULL,
                    1,
                    SYSUTCDATETIME(),
                    NULL
                FROM TBL_USUARIO u
                WHERE EXISTS (
                    SELECT 1
                    FROM TBL_PRODUTOS p
                    WHERE p.LojaId = u.Id
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM TBL_LOJA l
                    WHERE l.UsuarioId = u.Id
                );

                UPDATE p
                SET LojaId = l.Id
                FROM TBL_PRODUTOS p
                INNER JOIN TBL_LOJA l ON l.UsuarioId = p.LojaId;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_LOJA_LojaId",
                table: "TBL_PRODUTOS",
                column: "LojaId",
                principalTable: "TBL_LOJA",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_LOJA_LojaId",
                table: "TBL_PRODUTOS");

            migrationBuilder.Sql(
                """
                UPDATE p
                SET LojaId = l.UsuarioId
                FROM TBL_PRODUTOS p
                INNER JOIN TBL_LOJA l ON l.Id = p.LojaId;
                """);

            migrationBuilder.RenameColumn(
                name: "LojaId",
                table: "TBL_PRODUTOS",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_TBL_PRODUTOS_LojaId",
                table: "TBL_PRODUTOS",
                newName: "IX_TBL_PRODUTOS_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PRODUTOS_TBL_USUARIO_UsuarioId",
                table: "TBL_PRODUTOS",
                column: "UsuarioId",
                principalTable: "TBL_USUARIO",
                principalColumn: "Id");
        }
    }
}
