using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Infrastructure;

public class AppDbContextTests
{
    [Fact]
    public void Model_AppliesEmpenhoConfiguration_WhenBuilt()
    {
        using var context = InMemoryDbContextFactory.Create();

        var empenho = context.Model.FindEntityType(typeof(Empenho))!;

        empenho.GetTableName().Should().Be("empenhos");
        empenho.FindProperty(nameof(Empenho.NumeroEmpenho))!.GetMaxLength().Should().Be(50);
        empenho.FindProperty(nameof(Empenho.CnpjCredor))!.GetMaxLength().Should().Be(14);
        empenho.FindProperty(nameof(Empenho.Valor))!
            .FindAnnotation("Relational:ColumnType")!
            .Value.Should().Be("numeric(18,2)");
    }

    [Fact]
    public void Model_AppliesContratoConfiguration_WhenBuilt()
    {
        using var context = InMemoryDbContextFactory.Create();

        var contrato = context.Model.FindEntityType(typeof(Contrato))!;

        contrato.GetTableName().Should().Be("contratos");
        contrato.FindProperty(nameof(Contrato.NumeroContrato))!.GetMaxLength().Should().Be(50);
        contrato.FindProperty(nameof(Contrato.Objeto))!.GetMaxLength().Should().Be(1000);
    }

    [Fact]
    public void Model_AppliesOrgaoGovernoConfiguration_WhenBuilt()
    {
        using var context = InMemoryDbContextFactory.Create();

        var orgao = context.Model.FindEntityType(typeof(OrgaoGoverno))!;

        orgao.GetTableName().Should().Be("orgaos_governo");
        orgao.FindProperty(nameof(OrgaoGoverno.Codigo))!.GetMaxLength().Should().Be(20);
        orgao.FindProperty(nameof(OrgaoGoverno.Nome))!.GetMaxLength().Should().Be(200);
    }

    [Fact]
    public void Model_AppliesLiquidacaoConfiguration_WhenBuilt()
    {
        using var context = InMemoryDbContextFactory.Create();

        var liquidacao = context.Model.FindEntityType(typeof(Liquidacao))!;

        liquidacao.GetTableName().Should().Be("liquidacoes");
        liquidacao.FindProperty(nameof(Liquidacao.NumeroLiquidacao))!.GetMaxLength().Should().Be(50);
    }

    [Fact]
    public void Model_AppliesPagamentoConfiguration_WhenBuilt()
    {
        using var context = InMemoryDbContextFactory.Create();

        var pagamento = context.Model.FindEntityType(typeof(Pagamento))!;

        pagamento.GetTableName().Should().Be("pagamentos");
        pagamento.FindProperty(nameof(Pagamento.NumeroPagamento))!.GetMaxLength().Should().Be(50);
    }

    [Fact]
    public async Task SaveChangesAsync_SetsCreatedAt_WhenEntityIsAdded()
    {
        using var context = InMemoryDbContextFactory.Create();
        var beforeSave = DateTime.UtcNow.AddSeconds(-1);
        var orgao = new OrgaoGoverno
        {
            CreatedAt = DateTime.UnixEpoch,
            Codigo = "001",
            Nome = "Secretaria de Educacao",
            Sigla = "SEDUC",
            Tipo = "Secretaria"
        };

        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        orgao.CreatedAt.Should().BeAfter(beforeSave);
        orgao.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_SetsUpdatedAt_WhenEntityIsModified()
    {
        using var context = InMemoryDbContextFactory.Create();
        var orgao = new OrgaoGoverno
        {
            Codigo = "001",
            Nome = "Secretaria de Educacao",
            Sigla = "SEDUC",
            Tipo = "Secretaria"
        };
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);
        orgao.Nome = "Secretaria Estadual de Educacao";
        context.OrgaosGoverno.Update(orgao);
        await context.SaveChangesAsync();

        orgao.UpdatedAt.Should().NotBeNull();
        orgao.UpdatedAt!.Value.Should().BeAfter(beforeUpdate);
    }
}
