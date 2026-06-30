using FluentAssertions;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Entities;

public class ReceitaTests
{
    [Fact]
    public void Receita_AssignsAccountingFields_WhenInitialized()
    {
        // Act
        var receita = new Receita
        {
            Valor = 150000m,
            Mes = 1,
            Ano = 2026,
            Origem = "Impostos"
        };

        // Assert
        receita.Valor.Should().Be(150000m);
        receita.Mes.Should().Be(1);
        receita.Ano.Should().Be(2026);
        receita.Origem.Should().Be("Impostos");
    }

    [Fact]
    public void Receita_AssignsOrgaoGovernoId_WhenInitialized()
    {
        // Arrange
        var orgaoGovernoId = Guid.NewGuid();

        // Act
        var receita = new Receita
        {
            OrgaoGovernoId = orgaoGovernoId
        };

        // Assert
        receita.OrgaoGovernoId.Should().Be(orgaoGovernoId);
    }

    [Fact]
    public void Receita_GeneratesId_WhenCreated()
    {
        // Act
        var receita = new Receita();

        // Assert
        receita.Id.Should().NotBeEmpty();
    }
}
