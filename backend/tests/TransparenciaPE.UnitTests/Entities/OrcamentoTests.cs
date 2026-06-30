using FluentAssertions;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Entities;

public class OrcamentoTests
{
    [Fact]
    public void Orcamento_AssignsBudgetFields_WhenInitialized()
    {
        // Act
        var orcamento = new Orcamento
        {
            Ano = 2026,
            DotacaoInicial = 450000m,
            DotacaoAtualizada = 500000m
        };

        // Assert
        orcamento.Ano.Should().Be(2026);
        orcamento.DotacaoInicial.Should().Be(450000m);
        orcamento.DotacaoAtualizada.Should().Be(500000m);
    }

    [Fact]
    public void Orcamento_AssignsOrgaoGovernoId_WhenInitialized()
    {
        // Arrange
        var orgaoGovernoId = Guid.NewGuid();

        // Act
        var orcamento = new Orcamento
        {
            OrgaoGovernoId = orgaoGovernoId
        };

        // Assert
        orcamento.OrgaoGovernoId.Should().Be(orgaoGovernoId);
    }

    [Fact]
    public void Orcamento_GeneratesId_WhenCreated()
    {
        // Act
        var orcamento = new Orcamento();

        // Assert
        orcamento.Id.Should().NotBeEmpty();
    }
}
