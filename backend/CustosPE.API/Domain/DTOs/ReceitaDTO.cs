namespace CustosPE.API.Domain.DTOs;

public class ReceitaDTO
{
    public int Id { get; set; }
    public int OrgaoId { get; set; }
    public string NomeOrgao { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int Mes { get; set; }
    public decimal ValorArrecadado { get; set; }
    public decimal ValorPrevisto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Fonte { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}

public class CreateReceitaDTO
{
    public int OrgaoId { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
    public decimal ValorArrecadado { get; set; }
    public decimal ValorPrevisto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Fonte { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
