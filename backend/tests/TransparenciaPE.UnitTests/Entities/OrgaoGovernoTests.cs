using FluentAssertions;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Entities;

public class OrgaoGovernoTests
{
    [Fact]
    public void OrgaoGoverno_Should_Generate_UniqueId_OnCreation()
    {
        // Act
        var orgao1 = new OrgaoGoverno();
        var orgao2 = new OrgaoGoverno();

        // Assert
        orgao1.Id.Should().NotBeEmpty();
        orgao2.Id.Should().NotBeEmpty();
        orgao1.Id.Should().NotBe(orgao2.Id);
    }

    [Fact]
    public void OrgaoGoverno_Should_Initialize_Collections_AsEmpty()
    {
        // Act
        var orgao = new OrgaoGoverno();

        // Assert
        orgao.Empenhos.Should().NotBeNull().And.BeEmpty();
        orgao.Contratos.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void OrgaoGoverno_Should_Set_CreatedAt_ToUtcNow_OnCreation()
    {
        // Arrange
        var antes = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var orgao = new OrgaoGoverno();
        var depois = DateTime.UtcNow.AddSeconds(1);

        // Assert
        orgao.CreatedAt.Should().BeAfter(antes).And.BeBefore(depois);
        orgao.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void OrgaoGoverno_Should_Assign_Codigo_Nome_Sigla_Tipo()
    {
        // Act
        var orgao = new OrgaoGoverno
        {
            Codigo = "001",
            Nome = "Secretaria de Educação",
            Sigla = "SEDUC",
            Tipo = "Secretaria"
        };

        // Assert
        orgao.Codigo.Should().Be("001");
        orgao.Nome.Should().Be("Secretaria de Educação");
        orgao.Sigla.Should().Be("SEDUC");
        orgao.Tipo.Should().Be("Secretaria");
    }

    [Fact]
    public void OrgaoGoverno_Should_Have_TotalServidores_And_OrcamentoAtual()
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
