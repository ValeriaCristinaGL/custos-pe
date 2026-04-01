using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.Infrastructure.QueryServices;

/// <summary>
/// High-performance dashboard queries using Dapper for PostgreSQL (RQ001).
/// </summary>
public class DapperDashboardQueryService : IDashboardQueryService
{
    private readonly string _connectionString;

    public DapperDashboardQueryService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    public async Task<DashboardResumoResult> GetResumoAsync(int? ano = null)
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT
                COALESCE(SUM(e.""Valor""), 0) AS ""TotalEmpenhado"",
                COALESCE((SELECT SUM(l.""Valor"") FROM liquidacoes l
                    INNER JOIN empenhos e2 ON l.""EmpenhoId"" = e2.""Id""
                    WHERE (@Ano IS NULL OR e2.""Ano"" = @Ano)), 0) AS ""TotalLiquidado"",
                COALESCE((SELECT SUM(p.""Valor"") FROM pagamentos p
                    INNER JOIN liquidacoes l2 ON p.""LiquidacaoId"" = l2.""Id""
                    INNER JOIN empenhos e3 ON l2.""EmpenhoId"" = e3.""Id""
                    WHERE (@Ano IS NULL OR e3.""Ano"" = @Ano)), 0) AS ""TotalPago"",
                COUNT(e.""Id"") AS ""TotalEmpenhos"",
                (SELECT COUNT(c.""Id"") FROM contratos c) AS ""TotalContratos""
            FROM empenhos e
            WHERE (@Ano IS NULL OR e.""Ano"" = @Ano)";

        var result = await connection.QueryFirstOrDefaultAsync<DashboardResumoResult>(sql, new { Ano = ano });
        return result ?? new DashboardResumoResult();
    }

    public async Task<IEnumerable<ComparativoOrgaoResult>> GetComparativoOrgaosAsync(int ano)
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT
                o.""Codigo"" AS ""CodigoOrgao"",
                o.""Nome"" AS ""NomeOrgao"",
                o.""Sigla"" AS ""SiglaOrgao"",
                COALESCE(SUM(e.""Valor""), 0) AS ""TotalEmpenhado"",
                COALESCE(SUM(l_totals.""TotalLiq""), 0) AS ""TotalLiquidado"",
                COALESCE(SUM(p_totals.""TotalPag""), 0) AS ""TotalPago""
            FROM orgaos_governo o
            LEFT JOIN empenhos e ON e.""OrgaoGovernoId"" = o.""Id"" AND e.""Ano"" = @Ano
            LEFT JOIN LATERAL (
                SELECT COALESCE(SUM(l.""Valor""), 0) AS ""TotalLiq""
                FROM liquidacoes l WHERE l.""EmpenhoId"" = e.""Id""
            ) l_totals ON TRUE
            LEFT JOIN LATERAL (
                SELECT COALESCE(SUM(p.""Valor""), 0) AS ""TotalPag""
                FROM pagamentos p
                INNER JOIN liquidacoes l2 ON p.""LiquidacaoId"" = l2.""Id""
                WHERE l2.""EmpenhoId"" = e.""Id""
            ) p_totals ON TRUE
            GROUP BY o.""Codigo"", o.""Nome"", o.""Sigla""
            ORDER BY ""TotalEmpenhado"" DESC";

        return await connection.QueryAsync<ComparativoOrgaoResult>(sql, new { Ano = ano });
    }

    public async Task<IEnumerable<DrillDownResult>> GetDrillDownAsync(string codigoOrgao, int? ano = null)
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT
                e.""ClassificacaoMcasp"",
                e.""Descricao"",
                SUM(e.""Valor"") AS ""TotalEmpenhado"",
                COUNT(e.""Id"") AS ""QuantidadeEmpenhos""
            FROM empenhos e
            INNER JOIN orgaos_governo o ON e.""OrgaoGovernoId"" = o.""Id""
            WHERE o.""Codigo"" = @CodigoOrgao
                AND (@Ano IS NULL OR e.""Ano"" = @Ano)
            GROUP BY e.""ClassificacaoMcasp"", e.""Descricao""
            ORDER BY ""TotalEmpenhado"" DESC";

        return await connection.QueryAsync<DrillDownResult>(sql, new { CodigoOrgao = codigoOrgao, Ano = ano });
    }
}
