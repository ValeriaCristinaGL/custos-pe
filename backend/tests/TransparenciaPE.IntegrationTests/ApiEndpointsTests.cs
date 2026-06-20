using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TransparenciaPE.Application.DTOs;

namespace TransparenciaPE.IntegrationTests;

public class ApiEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetResumo_ReturnsOkStatus_WhenAnoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/dashboard/resumo?ano=2025");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetResumo_ReturnsJsonContentType_WhenAnoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/dashboard/resumo?ano=2025");

        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetResumo_ReturnsDashboardResumoPayload_WhenAnoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/dashboard/resumo?ano=2025");

        var payload = await response.Content.ReadFromJsonAsync<DashboardResumoDto>();
        payload.Should().NotBeNull();
        payload!.TotalEmpenhado.Should().Be(1000m);
        payload.PercentualExecutado.Should().Be(60m);
    }

    [Fact]
    public async Task GetEvolucao_ReturnsOkStatus_WhenCodigoOrgaoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/dashboard/evolucao?codigoOrgao=001&ano=2025");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEvolucao_ReturnsDrillDownPayload_WhenCodigoOrgaoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/dashboard/evolucao?codigoOrgao=001&ano=2025");

        var payload = await response.Content.ReadFromJsonAsync<DrillDownDto>();
        payload.Should().NotBeNull();
        payload!.CodigoOrgao.Should().Be("001");
        payload.Itens.Should().ContainSingle();
    }

    [Fact]
    public async Task PesquisaGlobal_ReturnsBadRequestStatus_WhenTermoIsEmpty()
    {
        var response = await _client.GetAsync("/api/v1/pesquisa/global?termo=");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PesquisaGlobal_ReturnsErrorPayload_WhenTermoIsEmpty()
    {
        var response = await _client.GetAsync("/api/v1/pesquisa/global?termo=");

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var hasControllerMessage = json.RootElement.TryGetProperty("message", out var message) &&
            !string.IsNullOrWhiteSpace(message.GetString());
        var hasValidationErrors = json.RootElement.TryGetProperty("errors", out var errors) &&
            errors.ValueKind == JsonValueKind.Object &&
            errors.EnumerateObject().Any();

        (hasControllerMessage || hasValidationErrors).Should().BeTrue();
    }

    [Fact]
    public async Task ExportarCsv_ReturnsOkStatus_WhenAnoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/exportar/csv?ano=2025");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ExportarCsv_ReturnsCsvContentType_WhenAnoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/exportar/csv?ano=2025");

        response.Content.Headers.ContentType!.MediaType.Should().Be("text/csv");
    }

    [Fact]
    public async Task ExportarCsv_ReturnsCsvHeader_WhenAnoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/exportar/csv?ano=2025");

        var csv = await response.Content.ReadAsStringAsync();
        csv.Should().StartWith("NumeroEmpenho;Orgao;Valor");
    }

    [Fact]
    public async Task ExportarCsv_ReturnsCsvFileName_WhenAnoIsProvided()
    {
        var response = await _client.GetAsync("/api/v1/exportar/csv?ano=2025");

        response.Content.Headers.ContentDisposition!.FileName.Should().Contain(".csv");
    }
}
