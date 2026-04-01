namespace TransparenciaPE.Application.DTOs;

public class PesquisaResultDto
{
    public IEnumerable<PesquisaItem> Resultados { get; set; } = Enumerable.Empty<PesquisaItem>();
    public int TotalResultados { get; set; }
    public string TermoBuscado { get; set; } = string.Empty;
}

public class PesquisaItem
{
    public string Tipo { get; set; } = string.Empty; // "Empenho" ou "Contrato"
    public string Numero { get; set; } = string.Empty;
    public string OrgaoNome { get; set; } = string.Empty;
    public string Fornecedor { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
