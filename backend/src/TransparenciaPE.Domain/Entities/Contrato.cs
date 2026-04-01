namespace TransparenciaPE.Domain.Entities;

public class Contrato : BaseEntity
{
    public string NumeroContrato { get; set; } = string.Empty;
    public Guid OrgaoGovernoId { get; set; }
    public OrgaoGoverno OrgaoGoverno { get; set; } = null!;
    public string Fornecedor { get; set; } = string.Empty;
    public string CnpjFornecedor { get; set; } = string.Empty;
    public decimal ValorContrato { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string Objeto { get; set; } = string.Empty;
}
