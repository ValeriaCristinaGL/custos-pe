using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransparenciaPE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReceitaOrcamentoAndOrgaoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrcamentoAtual",
                table: "orgaos_governo",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalServidores",
                table: "orgaos_governo",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Orcamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    DotacaoInicial = table.Column<decimal>(type: "numeric", nullable: false),
                    DotacaoAtualizada = table.Column<decimal>(type: "numeric", nullable: false),
                    OrgaoGovernoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orcamentos_orgaos_governo_OrgaoGovernoId",
                        column: x => x.OrgaoGovernoId,
                        principalTable: "orgaos_governo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receitas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Origem = table.Column<string>(type: "text", nullable: false),
                    OrgaoGovernoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receitas_orgaos_governo_OrgaoGovernoId",
                        column: x => x.OrgaoGovernoId,
                        principalTable: "orgaos_governo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_OrgaoGovernoId",
                table: "Orcamentos",
                column: "OrgaoGovernoId");

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_OrgaoGovernoId",
                table: "Receitas",
                column: "OrgaoGovernoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orcamentos");

            migrationBuilder.DropTable(
                name: "Receitas");

            migrationBuilder.DropColumn(
                name: "OrcamentoAtual",
                table: "orgaos_governo");

            migrationBuilder.DropColumn(
                name: "TotalServidores",
                table: "orgaos_governo");
        }
    }
}
