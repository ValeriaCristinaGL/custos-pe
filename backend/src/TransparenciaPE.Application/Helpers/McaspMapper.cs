namespace TransparenciaPE.Application.Helpers;

public static class McaspMapper
{
    public static string MapToClassificacao(string naturezaDaDespesa, string descricao = "")
    {
        if (string.IsNullOrWhiteSpace(naturezaDaDespesa))
            return "Outros";

        if (naturezaDaDespesa.StartsWith("3.1"))
        {
            return "Pessoal e Encargos Sociais";
        }
        else if (naturezaDaDespesa.StartsWith("3.3"))
        {
            return "Custeio";
        }
        else if (naturezaDaDespesa.StartsWith("4.4"))
        {
            return "Investimentos";
        }

        return "Outros";
    }
}
