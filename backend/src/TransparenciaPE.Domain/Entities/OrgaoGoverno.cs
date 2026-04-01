namespace TransparenciaPE.Domain.Entities;

public class OrgaoGoverno : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // Secretaria, Autarquia, etc.

    public ICollection<Empenho> Empenhos { get; set; } = new List<Empenho>();
    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
}
