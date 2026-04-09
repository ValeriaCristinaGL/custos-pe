namespace TransparenciaPE.Application.DTOs;

public class ComparativoOrgaosDto
{
    public IEnumerable<OrgaoComparativoItem> Orgaos { get; set; } = Enumerable.Empty<OrgaoComparativoItem>();
    public int Ano { get; set; }
}

public class OrgaoComparativoItem
{
    public string CodigoOrgao { get; set; } = string.Empty;
    public string NomeOrgao { get; set; } = string.Empty;
    public string SiglaOrgao { get; set; } = string.Empty;
    public decimal TotalEmpenhado { get; set; }
    public decimal TotalLiquidado { get; set; }
    public decimal TotalPago { get; set; }
}
