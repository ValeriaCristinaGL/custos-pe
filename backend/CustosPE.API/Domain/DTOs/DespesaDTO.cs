namespace CustosPE.API.Domain.DTOs;

public class DespesaDTO
{
    public int Id { get; set; }
    public int OrgaoId { get; set; }
    public string NomeOrgao { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int Mes { get; set; }
    public decimal ValorEmpenhado { get; set; }
    public decimal ValorLiquidado { get; set; }
    public decimal ValorPago { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
    public string? Subfuncao { get; set; }
    public string? Descricao { get; set; }
}

public class CreateDespesaDTO
{
    public int OrgaoId { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
    public decimal ValorEmpenhado { get; set; }
    public decimal ValorLiquidado { get; set; }
    public decimal ValorPago { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
    public string? Subfuncao { get; set; }
    public string? Descricao { get; set; }
}
