namespace TransparenciaPE.Domain.Entities;

public class Receita : BaseEntity
{
    public decimal Valor { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string Origem { get; set; } = string.Empty;

    public Guid OrgaoGovernoId { get; set; }
    public OrgaoGoverno OrgaoGoverno { get; set; } = null!;
}
