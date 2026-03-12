namespace CustosPE.API.Domain.Entities;

public class Orgao
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Sigla { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
    public ICollection<Receita> Receitas { get; set; } = new List<Receita>();
}
