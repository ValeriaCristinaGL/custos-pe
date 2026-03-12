namespace CustosPE.API.Domain.Entities;

public class Receita
{
    public int Id { get; set; }
    public int OrgaoId { get; set; }
    public Orgao Orgao { get; set; } = null!;
    public int Ano { get; set; }
    public int Mes { get; set; }
    public decimal ValorArrecadado { get; set; }
    public decimal ValorPrevisto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Fonte { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
