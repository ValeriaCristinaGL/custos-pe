using FluentAssertions;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Entities;

public class OrcamentoTests
{
    [Fact]
    public void Orcamento_Should_Be_Instantiated_And_Have_Expected_Properties()
    {
        // Act
        var orcamento = new Orcamento
        {
            Ano = 2026,
            DotacaoInicial = 450000m,
            DotacaoAtualizada = 500000m,
            OrgaoGovernoId = Guid.NewGuid()
        };

        // Assert
        orcamento.Ano.Should().Be(2026);
        orcamento.DotacaoInicial.Should().Be(450000m);
        orcamento.DotacaoAtualizada.Should().Be(500000m);
        orcamento.OrgaoGovernoId.Should().NotBeEmpty();
        orcamento.Id.Should().NotBeEmpty();
    }
}
