using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaEnumsTcc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [TBL_LOJA_ENTREGA_OPCAO]
                SET [TipoEntregaId] = CASE [TipoEntregaId]
                    WHEN 4 THEN 3
                    WHEN 5 THEN 4
                    ELSE [TipoEntregaId]
                END
                WHERE [TipoEntregaId] IN (4, 5);
                """);

            migrationBuilder.Sql(
                """
                UPDATE [TBL_PEDIDO]
                SET [TipoEntregaId] = CASE [TipoEntregaId]
                    WHEN 4 THEN 3
                    WHEN 5 THEN 4
                    ELSE [TipoEntregaId]
                END
                WHERE [TipoEntregaId] IN (4, 5);
                """);

            migrationBuilder.Sql(
                """
                UPDATE [TBL_VENDA]
                SET [StatusVenda] = CASE [StatusVenda]
                    WHEN 2 THEN 1
                    WHEN 3 THEN 2
                    WHEN 4 THEN 2
                    WHEN 5 THEN 3
                    WHEN 6 THEN 4
                    WHEN 7 THEN 5
                    WHEN 8 THEN 3
                    WHEN 9 THEN 3
                    ELSE [StatusVenda]
                END
                WHERE [StatusVenda] IN (2, 3, 4, 5, 6, 7, 8, 9);
                """);

            migrationBuilder.Sql(
                """
                UPDATE [TBL_USUARIO_CARTAO_PAGAMENTO]
                SET [Tipo] = 2
                WHERE [Tipo] IN (3, 4);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [TBL_LOJA_ENTREGA_OPCAO]
                SET [TipoEntregaId] = CASE [TipoEntregaId]
                    WHEN 4 THEN 5
                    ELSE [TipoEntregaId]
                END
                WHERE [TipoEntregaId] = 4;
                """);

            migrationBuilder.Sql(
                """
                UPDATE [TBL_PEDIDO]
                SET [TipoEntregaId] = CASE [TipoEntregaId]
                    WHEN 4 THEN 5
                    ELSE [TipoEntregaId]
                END
                WHERE [TipoEntregaId] = 4;
                """);

            migrationBuilder.Sql(
                """
                UPDATE [TBL_VENDA]
                SET [StatusVenda] = CASE [StatusVenda]
                    WHEN 1 THEN 1
                    WHEN 2 THEN 3
                    WHEN 3 THEN 9
                    WHEN 4 THEN 6
                    WHEN 5 THEN 7
                    ELSE [StatusVenda]
                END
                WHERE [StatusVenda] IN (1, 2, 3, 4, 5);
                """);
        }
    }
}
