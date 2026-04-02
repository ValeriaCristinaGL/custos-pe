using FluentAssertions;
using TransparenciaPE.Application.Helpers;

namespace TransparenciaPE.UnitTests.Helpers;

public class McaspMapperTests
{
    [Theory]
    [InlineData("3.1.90.11", true, false, false)] // Pessoal e Encargos Sociais
    [InlineData("3.3.90.30", false, true, false)] // Outras Despesas Correntes (Custeio)
    [InlineData("4.4.90.51", false, false, true)] // Investimentos
    [InlineData("4.6.90.71", false, false, false)] // Amortização (não é Pessoal, Custeio ou Investimento primário nos dashboards)
    [InlineData("10.0.00.00", false, false, false)] // Inválido/Outros
    [InlineData("", false, false, false)] // Vazio
    public void ClassificarDespesa_Returns_CorrectFlags(string natureza, bool isPessoal, bool isCusteio, bool isInvestimento)
    {
        // Act
        var classificacao = McaspMapper.ClassificarDespesa(natureza);

        // Assert
        classificacao.IsPessoal.Should().Be(isPessoal);
        classificacao.IsCusteio.Should().Be(isCusteio);
        classificacao.IsInvestimento.Should().Be(isInvestimento);
    }
}
