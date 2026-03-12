namespace CustosPE.API.Domain.DTOs;

public class DashboardDTO
{
    public decimal TotalDespesas { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
    public int TotalOrgaos { get; set; }
    public int AnoReferencia { get; set; }
    public List<DespesaPorCategoriaDTO> DespesasPorCategoria { get; set; } = new();
    public List<ReceitaPorCategoriaDTO> ReceitasPorCategoria { get; set; } = new();
    public List<DespesaPorOrgaoDTO> TopOrgaosPorDespesa { get; set; } = new();
    public List<EvolucaoMensalDTO> EvolucaoMensal { get; set; } = new();
}

public class DespesaPorCategoriaDTO
{
    public string Categoria { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public decimal Percentual { get; set; }
}

public class ReceitaPorCategoriaDTO
{
    public string Categoria { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public decimal Percentual { get; set; }
}

public class DespesaPorOrgaoDTO
{
    public string NomeOrgao { get; set; } = string.Empty;
    public string SiglaOrgao { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
}

public class EvolucaoMensalDTO
{
    public int Mes { get; set; }
    public string NomeMes { get; set; } = string.Empty;
    public decimal TotalDespesas { get; set; }
    public decimal TotalReceitas { get; set; }
}
