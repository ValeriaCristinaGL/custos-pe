namespace TransparenciaPE.Domain.Entities;

public class Liquidacao : BaseEntity
{
    public string NumeroLiquidacao { get; set; } = string.Empty;
    public Guid EmpenhoId { get; set; }
    public Empenho Empenho { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime DataLiquidacao { get; set; }

    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
