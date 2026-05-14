using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TransparenciaPE.Domain.Interfaces;
using TransparenciaPE.Infrastructure.ExternalClients;

namespace TransparenciaPE.UnitTests.ExternalClients;

public class TcePEDataClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<TcePEDataClient>> _loggerMock;
    private readonly TcePEDataClient _client;

    public TcePEDataClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://sistemas.tce.pe.gov.br/")
        };
        _loggerMock = new Mock<ILogger<TcePEDataClient>>();

        _client = new TcePEDataClient(_httpClient, _loggerMock.Object);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, object? body = null)
    {
        var json = body is null ? "{}" : JsonSerializer.Serialize(body);
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(json)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);
    }

    // ─── Receitas ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetReceitasAsync_Should_Deserialize_Tce_Json_Format_Correctly()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, new
        {
            resposta = new
            {
                status = "OK",
                conteudo = new[]
                {
                    new { ValorReceita = 150000m, Mes = 1, Ano = 2026, Origem = "Imposto", CodigoOrgao = "SEE" }
                }
            }
        });

        // Act
        var result = await _client.GetReceitasAsync(2026);

        // Assert
        result.Should().NotBeNull().And.HaveCount(1);
        result.First().ValorReceita.Should().Be(150000m);
        result.First().Origem.Should().Be("Imposto");
    }

    [Fact]
    public async Task GetReceitasAsync_ShouldThrow_WhenApiReturnsServerError()
    {
        // Arrange — GetReceitasAsync usa EnsureSuccessStatusCode, então lança exceção
        SetupHttpResponse(HttpStatusCode.InternalServerError);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => _client.GetReceitasAsync(2026));
    }

    // ─── EmpenhosByOrgao ─────────────────────────────────────────────────

    [Fact]
    public async Task GetEmpenhosByOrgaoAsync_Should_Deserialize_Correctly()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, new
        {
            resposta = new
            {
                status = "OK",
                conteudo = new[]
                {
                    new
                    {
                        NumeroEmpenho = "EMP-001",
                        Ano = 2025,
                        CodigoOrgao = "001",
                        NomeOrgao = "Sec. Educação",
                        SiglaOrgao = "SEDUC",
                        Credor = "Empresa ABC",
                        CnpjCredor = "11222333000181",
                        Valor = 50000m,
                        DataEmpenho = "2025-03-15",
                        NaturezaDespesa = "3.3.90.30"
                    }
                }
            }
        });

        // Act
        var result = await _client.GetEmpenhosByOrgaoAsync(2025, "001");

        // Assert
        result.Should().NotBeNull().And.HaveCount(1);
        result.First().NumeroEmpenho.Should().Be("EMP-001");
        result.First().Valor.Should().Be(50000m);
        result.First().NaturezaDespesa.Should().Be("3.3.90.30");
    }

    [Fact]
    public async Task GetEmpenhosByOrgaoAsync_ShouldReturnEmpty_WhenApiReturnsError()
    {
        // Arrange — métodos com verificação manual retornam lista vazia em caso de erro
        SetupHttpResponse(HttpStatusCode.NotFound);

        // Act
        var result = await _client.GetEmpenhosByOrgaoAsync(2025, "999");

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }

    // ─── Contratos ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetContratosAsync_Should_Deserialize_Correctly()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, new
        {
            resposta = new
            {
                status = "OK",
                conteudo = new[]
                {
                    new
                    {
                        NumeroContrato = "CT-2025-001",
                        CodigoOrgao = "002",
                        NomeOrgao = "Sec. Saúde",
                        Fornecedor = "Farmácia Central",
                        CnpjFornecedor = "22333444000155",
                        ValorContrato = 200000m,
                        DataInicio = "2025-01-01",
                        Objeto = "Fornecimento de medicamentos"
                    }
                }
            }
        });

        // Act
        var result = await _client.GetContratosAsync(2025);

        // Assert
        result.Should().NotBeNull().And.HaveCount(1);
        result.First().NumeroContrato.Should().Be("CT-2025-001");
        result.First().ValorContrato.Should().Be(200000m);
    }

    [Fact]
    public async Task GetContratosAsync_ShouldReturnEmpty_WhenApiReturnsError()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.ServiceUnavailable);

        // Act
        var result = await _client.GetContratosAsync(2025);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }

    // ─── Orcamento ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrcamentoAsync_ShouldReturnEmpty_WhenApiReturnsError()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.BadGateway);

        // Act
        var result = await _client.GetOrcamentoAsync(2025);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }
}
