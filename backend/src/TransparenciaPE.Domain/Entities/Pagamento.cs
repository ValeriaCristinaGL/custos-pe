namespace TransparenciaPE.Domain.Entities;

public class Pagamento : BaseEntity
{
    public string NumeroPagamento { get; set; } = string.Empty;
    public Guid LiquidacaoId { get; set; }
    public Liquidacao Liquidacao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
}
