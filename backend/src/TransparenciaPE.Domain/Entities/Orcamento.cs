namespace TransparenciaPE.Domain.Entities;

public class Orcamento : BaseEntity
{
    public int Ano { get; set; }
    public decimal DotacaoInicial { get; set; }
    public decimal DotacaoAtualizada { get; set; }

    public Guid OrgaoGovernoId { get; set; }
    public OrgaoGoverno OrgaoGoverno { get; set; } = null!;
}
