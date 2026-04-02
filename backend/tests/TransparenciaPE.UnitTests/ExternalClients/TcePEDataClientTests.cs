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

    [Fact]
    public async Task GetReceitasAsync_Should_Deserialize_Tce_Json_Format_Correctly()
    {
        // Arrange
        var tceResponse = new
        {
            resposta = new
            {
                status = "OK",
                conteudo = new[]
                {
                    new { ValorReceita = 150000m, Mes = 1, Ano = 2026, Origem = "Imposto", CodigoOrgao = "SEE" }
                }
            }
        };

        var json = JsonSerializer.Serialize(tceResponse);
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _client.GetReceitasAsync(2026);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().ValorReceita.Should().Be(150000m);
        result.First().Origem.Should().Be("Imposto");
    }
}
