using FluentAssertions;
using TransparenciaPE.Application.Helpers;

namespace TransparenciaPE.UnitTests.Helpers;

public class McaspMapperTests
{
    [Theory]
    [InlineData("3.1.90.11", "Pessoal e Encargos Sociais")]
    [InlineData("3.3.90.30", "Custeio")]
    [InlineData("4.4.90.51", "Investimentos")]
    [InlineData("4.6.90.71", "Outros")]
    [InlineData("10.0.00.00", "Outros")]
    [InlineData("", "Outros")]
    public void ClassificarDespesa_Returns_CorrectString(string natureza, string expected)
    {
        // Act
        var result = McaspMapper.MapToClassificacao(natureza, string.Empty);

        // Assert
        result.Should().Be(expected);
    }
}
