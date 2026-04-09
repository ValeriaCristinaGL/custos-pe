using FluentAssertions;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Entities;

public class OrgaoGovernoTests
{
    [Fact]
    public void OrgaoGoverno_Should_Have_New_Properties()
    {
        // Act
        var orgao = new OrgaoGoverno
        {
            TotalServidores = 1500,
            OrcamentoAtual = 5000000m
        };

        // Assert
        orgao.TotalServidores.Should().Be(1500);
        orgao.OrcamentoAtual.Should().Be(5000000m);
    }
}
