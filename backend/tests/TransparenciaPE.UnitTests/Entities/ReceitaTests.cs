using FluentAssertions;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Entities;

public class ReceitaTests
{
    [Fact]
    public void Receita_Should_Be_Instantiated_And_Have_Expected_Properties()
    {
        // Act
        var receita = new Receita
        {
            Valor = 150000m,
            Mes = 1,
            Ano = 2026,
            Origem = "Impostos",
            OrgaoGovernoId = Guid.NewGuid()
        };

        // Assert
        receita.Valor.Should().Be(150000m);
        receita.Mes.Should().Be(1);
        receita.Ano.Should().Be(2026);
        receita.Origem.Should().Be("Impostos");
        receita.OrgaoGovernoId.Should().NotBeEmpty();
        receita.Id.Should().NotBeEmpty();
    }
}
