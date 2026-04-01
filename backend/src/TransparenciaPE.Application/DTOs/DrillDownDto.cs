namespace TransparenciaPE.Application.DTOs;

public class DrillDownDto
{
    public string CodigoOrgao { get; set; } = string.Empty;
    public string NomeOrgao { get; set; } = string.Empty;
    public IEnumerable<DrillDownItem> Itens { get; set; } = Enumerable.Empty<DrillDownItem>();
}

public class DrillDownItem
{
    public string ClassificacaoMcasp { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal TotalEmpenhado { get; set; }
    public int QuantidadeEmpenhos { get; set; }
}
