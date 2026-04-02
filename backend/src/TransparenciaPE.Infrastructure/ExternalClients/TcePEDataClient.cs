using System.Text.Json;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.Infrastructure.ExternalClients;

public class TcePEDataClient : IPEDataClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TcePEDataClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public TcePEDataClient(HttpClient httpClient, ILogger<TcePEDataClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosAsync(int ano) => throw new NotImplementedException();
    
    public Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosByOrgaoAsync(int ano, string codigoOrgao) => throw new NotImplementedException();
    
    public Task<IEnumerable<ExternalContratoData>> GetContratosAsync(int ano) => throw new NotImplementedException();
    
    public async Task<IEnumerable<ExternalReceitaData>> GetReceitasAsync(int ano)
    {
        var response = await _httpClient.GetAsync($"DadosAbertos/ReceitasEstaduais?ano={ano}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tceResponse = JsonSerializer.Deserialize<TceWrapper<ExternalReceitaData>>(json, _jsonOptions);

        return tceResponse?.Resposta?.Conteudo ?? Enumerable.Empty<ExternalReceitaData>();
    }

    public Task<IEnumerable<ExternalOrcamentoData>> GetOrcamentoAsync(int ano) => throw new NotImplementedException();

    public Task<int> GetTotalServidoresAsync(string codigoOrgao) => throw new NotImplementedException();
}

public class TceWrapper<T>
{
    public TceResposta<T> Resposta { get; set; } = new();
}

public class TceResposta<T>
{
    public string Status { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public List<T> Conteudo { get; set; } = new();
}
