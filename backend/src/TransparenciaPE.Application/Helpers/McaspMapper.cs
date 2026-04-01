namespace TransparenciaPE.Application.Helpers;

public static class McaspMapper
{
    /// <summary>
    /// Maps raw fiscal data to MCASP classification string.
    /// </summary>
    public static string MapToClassificacao(string naturezaDespesa, string descricao)
    {
        var hasNatureza = !string.IsNullOrWhiteSpace(naturezaDespesa);
        var hasDescricao = !string.IsNullOrWhiteSpace(descricao);

        if (!hasNatureza && !hasDescricao)
            return "Não Classificado";

        if (!hasNatureza)
            return $"Não Classificado - {descricao}";

        if (!hasDescricao)
            return $"{naturezaDespesa} - Sem Descrição";

        return $"{naturezaDespesa} - {descricao}";
    }

    /// <summary>
    /// Determines if a nature code represents an expense (MCASP categories 3 and 4).
    /// </summary>
    public static bool IsDespesa(string naturezaDespesa)
    {
        if (string.IsNullOrWhiteSpace(naturezaDespesa))
            return false;

        var firstChar = naturezaDespesa.TrimStart()[0];
        return firstChar is '3' or '4';
    }
}
