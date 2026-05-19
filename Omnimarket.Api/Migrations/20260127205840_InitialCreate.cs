using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_USUARIO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cpf = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Sobrenome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Foto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DataAcesso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_USUARIO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ENDERECO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoLogradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NomeEndereco = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Complemento = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Cep = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Cidade = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Uf = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ENDERECO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_ENDERECO_TBL_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_TELEFONE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    NumeroE164 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_TELEFONE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_TELEFONE_TBL_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TBL_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ENDERECO_UsuarioId",
                table: "TBL_ENDERECO",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_TELEFONE_UsuarioId",
                table: "TBL_TELEFONE",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USUARIO_Cpf",
                table: "TBL_USUARIO",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USUARIO_Email",
                table: "TBL_USUARIO",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_ENDERECO");

            migrationBuilder.DropTable(
                name: "TBL_TELEFONE");

            migrationBuilder.DropTable(
                name: "TBL_USUARIO");
        }
    }
}
