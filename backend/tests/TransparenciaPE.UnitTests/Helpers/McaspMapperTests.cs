using TransparenciaPE.Application.Helpers;

namespace TransparenciaPE.UnitTests.Helpers;

public class McaspMapperTests
{
    [Theory]
    [InlineData("3.3.90.30", "Material de Consumo", "3.3.90.30 - Material de Consumo")]
    [InlineData("3.3.90.39", "Serviços de Terceiros - PJ", "3.3.90.39 - Serviços de Terceiros - PJ")]
    [InlineData("4.4.90.52", "Equipamentos e Material Permanente", "4.4.90.52 - Equipamentos e Material Permanente")]
    public void MapToClassificacao_ShouldCombineNaturezaAndDescription(
        string naturezaDespesa, string descricao, string expected)
    {
        // Act
        var result = McaspMapper.MapToClassificacao(naturezaDespesa, descricao);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MapToClassificacao_ShouldHandleEmptyNatureza()
    {
        // Act
        var result = McaspMapper.MapToClassificacao("", "Descricao");

        // Assert
        Assert.Equal("Não Classificado - Descricao", result);
    }

    [Fact]
    public void MapToClassificacao_ShouldHandleEmptyDescription()
    {
        // Act
        var result = McaspMapper.MapToClassificacao("3.3.90.30", "");

        // Assert
        Assert.Equal("3.3.90.30 - Sem Descrição", result);
    }

    [Fact]
    public void MapToClassificacao_ShouldHandleBothEmpty()
    {
        // Act
        var result = McaspMapper.MapToClassificacao("", "");

        // Assert
        Assert.Equal("Não Classificado", result);
    }

    [Theory]
    [InlineData("3", true)]   // Despesas Correntes
    [InlineData("4", true)]   // Despesas de Capital
    [InlineData("1", false)]  // Receita (não é despesa)
    [InlineData("", false)]
    public void IsDespesa_ShouldIdentifyExpenseCategories(string natureza, bool expected)
    {
        // Act
        var result = McaspMapper.IsDespesa(natureza);

        // Assert
        Assert.Equal(expected, result);
    }
}
