namespace TransparenciaPE.Domain.Interfaces;

/// <summary>
/// High-performance query service for Dashboard/BI endpoints using Dapper.
/// </summary>
public interface IDashboardQueryService
{
    Task<DashboardResumoResult> GetResumoAsync(int? ano = null);
    Task<IEnumerable<ComparativoOrgaoResult>> GetComparativoOrgaosAsync(int ano);
    Task<IEnumerable<DrillDownResult>> GetDrillDownAsync(string codigoOrgao, int? ano = null);
}

public class DashboardResumoResult
{
    public decimal TotalEmpenhado { get; set; }
    public decimal TotalLiquidado { get; set; }
    public decimal TotalPago { get; set; }
    public int TotalEmpenhos { get; set; }
    public int TotalContratos { get; set; }
}

public class ComparativoOrgaoResult
{
    public string CodigoOrgao { get; set; } = string.Empty;
    public string NomeOrgao { get; set; } = string.Empty;
    public string SiglaOrgao { get; set; } = string.Empty;
    public decimal TotalEmpenhado { get; set; }
    public decimal TotalLiquidado { get; set; }
    public decimal TotalPago { get; set; }
}

public class DrillDownResult
{
    public string ClassificacaoMcasp { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal TotalEmpenhado { get; set; }
    public int QuantidadeEmpenhos { get; set; }
}
