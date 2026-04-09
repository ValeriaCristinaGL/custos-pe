namespace TransparenciaPE.Domain.Entities;

public class Empenho : BaseEntity
{
    public string NumeroEmpenho { get; set; } = string.Empty;
    public int Ano { get; set; }
    public Guid OrgaoGovernoId { get; set; }
    public OrgaoGoverno OrgaoGoverno { get; set; } = null!;
    public string Credor { get; set; } = string.Empty;
    public string CnpjCredor { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataEmpenho { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string ClassificacaoMcasp { get; set; } = string.Empty;

    public ICollection<Liquidacao> Liquidacoes { get; set; } = new List<Liquidacao>();
}
