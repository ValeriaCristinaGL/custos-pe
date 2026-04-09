using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransparenciaPE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orgaos_governo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Sigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orgaos_governo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroContrato = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrgaoGovernoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fornecedor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CnpjFornecedor = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    ValorContrato = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Objeto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contratos_orgaos_governo_OrgaoGovernoId",
                        column: x => x.OrgaoGovernoId,
                        principalTable: "orgaos_governo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "empenhos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroEmpenho = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    OrgaoGovernoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Credor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CnpjCredor = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataEmpenho = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ClassificacaoMcasp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empenhos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_empenhos_orgaos_governo_OrgaoGovernoId",
                        column: x => x.OrgaoGovernoId,
                        principalTable: "orgaos_governo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "liquidacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroLiquidacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmpenhoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataLiquidacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_liquidacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_liquidacoes_empenhos_EmpenhoId",
                        column: x => x.EmpenhoId,
                        principalTable: "empenhos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroPagamento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LiquidacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagamentos_liquidacoes_LiquidacaoId",
                        column: x => x.LiquidacaoId,
                        principalTable: "liquidacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contratos_cnpj",
                table: "contratos",
                column: "CnpjFornecedor");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_numero",
                table: "contratos",
                column: "NumeroContrato",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contratos_OrgaoGovernoId",
                table: "contratos",
                column: "OrgaoGovernoId");

            migrationBuilder.CreateIndex(
                name: "ix_empenhos_cnpj",
                table: "empenhos",
                column: "CnpjCredor");

            migrationBuilder.CreateIndex(
                name: "ix_empenhos_data",
                table: "empenhos",
                column: "DataEmpenho");

            migrationBuilder.CreateIndex(
                name: "ix_empenhos_numero_ano",
                table: "empenhos",
                columns: new[] { "NumeroEmpenho", "Ano" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empenhos_OrgaoGovernoId",
                table: "empenhos",
                column: "OrgaoGovernoId");

            migrationBuilder.CreateIndex(
                name: "IX_liquidacoes_EmpenhoId",
                table: "liquidacoes",
                column: "EmpenhoId");

            migrationBuilder.CreateIndex(
                name: "ix_orgaos_codigo",
                table: "orgaos_governo",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_orgaos_nome",
                table: "orgaos_governo",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_LiquidacaoId",
                table: "pagamentos",
                column: "LiquidacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contratos");

            migrationBuilder.DropTable(
                name: "pagamentos");

            migrationBuilder.DropTable(
                name: "liquidacoes");

            migrationBuilder.DropTable(
                name: "empenhos");

            migrationBuilder.DropTable(
                name: "orgaos_governo");
        }
    }
}
