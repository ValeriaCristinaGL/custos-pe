namespace TransparenciaPE.Application.DTOs;

public class DashboardResumoDto
{
    public decimal TotalEmpenhado { get; set; }
    public decimal TotalLiquidado { get; set; }
    public decimal TotalPago { get; set; }
    public decimal PercentualExecutado { get; set; }
    public int TotalEmpenhos { get; set; }
    public int TotalContratos { get; set; }
}
