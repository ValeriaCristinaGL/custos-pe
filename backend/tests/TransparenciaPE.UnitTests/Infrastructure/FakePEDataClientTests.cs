using FluentAssertions;
using TransparenciaPE.Infrastructure.ExternalClients;

namespace TransparenciaPE.UnitTests.Infrastructure;

public class FakePEDataClientTests
{
    private readonly FakePEDataClient _client = new();

    [Fact]
    public async Task GetEmpenhosAsync_ReturnsSeedData_ForRequestedYear()
    {
        var result = (await _client.GetEmpenhosAsync(2025)).ToList();

        result.Should().HaveCount(3);
        result.Should().OnlyContain(e => e.Ano == 2025);
        result.Should().Contain(e => e.NumeroEmpenho == "EMP-2025-001");
    }

    [Fact]
    public async Task GetEmpenhosByOrgaoAsync_ReturnsFilteredData_ForRequestedOrgao()
    {
        var result = (await _client.GetEmpenhosByOrgaoAsync(2025, "SES")).ToList();

        result.Should().ContainSingle();
        result.Single().CodigoOrgao.Should().Be("SES");
    }

    [Fact]
    public async Task GetContratosAsync_ReturnsSeedData_ForRequestedYear()
    {
        var result = (await _client.GetContratosAsync(2025)).ToList();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.NumeroContrato.StartsWith("CT-2025"));
    }

    [Fact]
    public async Task GetReceitasAsync_ReturnsSeedData_ForRequestedYear()
    {
        var result = (await _client.GetReceitasAsync(2025)).ToList();

        result.Should().ContainSingle();
        result.Single().Ano.Should().Be(2025);
        result.Single().ValorReceita.Should().Be(1000000m);
    }

    [Fact]
    public async Task GetOrcamentoAsync_ReturnsSeedData_ForRequestedYear()
    {
        var result = (await _client.GetOrcamentoAsync(2025)).ToList();

        result.Should().ContainSingle();
        result.Single().Ano.Should().Be(2025);
        result.Single().ValorDotacaoAtualizada.Should().Be(6000000m);
    }

    [Fact]
    public async Task GetTotalServidoresAsync_ReturnsFixedTotal_ForAnyOrgao()
    {
        var result = await _client.GetTotalServidoresAsync("SEE");

        result.Should().Be(1200);
    }
}
