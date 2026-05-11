using FluentAssertions;
using TransparenciaPE.Application.Helpers;

namespace TransparenciaPE.UnitTests.Helpers;

public class McaspMapperTests
{
    [Theory]
    // Pessoal e Encargos Sociais (prefixo 3.1)
    [InlineData("3.1.90.11", "Pessoal e Encargos Sociais")]
    [InlineData("3.1.00.00", "Pessoal e Encargos Sociais")]
    // Custeio (prefixo 3.3)
    [InlineData("3.3.90.30", "Custeio")]
    [InlineData("3.3.90.39", "Custeio")]
    // Investimentos (prefixo 4.4)
    [InlineData("4.4.90.51", "Investimentos")]
    [InlineData("4.4.90.61", "Investimentos")]
    // Outros — prefixos não mapeados
    [InlineData("3.2.90.00", "Outros")]
    [InlineData("4.5.90.52", "Outros")]
    [InlineData("4.6.90.71", "Outros")]
    [InlineData("5.0.00.00", "Outros")]
    [InlineData("10.0.00.00", "Outros")]
    // Entradas inválidas/vazias
    [InlineData("", "Outros")]
    [InlineData("   ", "Outros")]
    public void MapToClassificacao_Returns_CorrectClassification(string natureza, string expected)
    {
        // Act
        var result = McaspMapper.MapToClassificacao(natureza, string.Empty);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void MapToClassificacao_Returns_Outros_WhenNaturezaIsNull()
    {
        // Act
        var result = McaspMapper.MapToClassificacao(null!, string.Empty);

        // Assert
        result.Should().Be("Outros");
    }
}
