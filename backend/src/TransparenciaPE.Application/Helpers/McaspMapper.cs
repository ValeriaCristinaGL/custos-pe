namespace TransparenciaPE.Application.Helpers;

public static class McaspMapper
{
    public static McaspClassificacao ClassificarDespesa(string naturezaDaDespesa)
    {
        var classificacao = new McaspClassificacao();

        if (string.IsNullOrWhiteSpace(naturezaDaDespesa))
            return classificacao;

        if (naturezaDaDespesa.StartsWith("3.1"))
        {
            classificacao.IsPessoal = true;
        }
        else if (naturezaDaDespesa.StartsWith("3.3"))
        {
            classificacao.IsCusteio = true;
        }
        else if (naturezaDaDespesa.StartsWith("4.4"))
        {
            classificacao.IsInvestimento = true;
        }

        return classificacao;
    }
}

public class McaspClassificacao
{
    public bool IsPessoal { get; set; }
    public bool IsCusteio { get; set; }
    public bool IsInvestimento { get; set; }
}
