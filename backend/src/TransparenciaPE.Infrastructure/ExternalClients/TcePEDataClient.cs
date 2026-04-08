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

    public async Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosAsync(int ano)
    {
        var response = await _httpClient.GetAsync($"DadosAbertos/EmpenhosEstaduais?ano={ano}");
        if (!response.IsSuccessStatusCode) return Enumerable.Empty<ExternalEmpenhoData>();
        var json = await response.Content.ReadAsStringAsync();
        var tceResponse = JsonSerializer.Deserialize<TceWrapper<ExternalEmpenhoData>>(json, _jsonOptions);
        return tceResponse?.Resposta?.Conteudo ?? Enumerable.Empty<ExternalEmpenhoData>();
    }
    
    public async Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosByOrgaoAsync(int ano, string codigoOrgao)
    {
        var response = await _httpClient.GetAsync($"DadosAbertos/EmpenhosEstaduais?ano={ano}&unidadeGestora={codigoOrgao}");
        if (!response.IsSuccessStatusCode) return Enumerable.Empty<ExternalEmpenhoData>();
        var json = await response.Content.ReadAsStringAsync();
        var tceResponse = JsonSerializer.Deserialize<TceWrapper<ExternalEmpenhoData>>(json, _jsonOptions);
        return tceResponse?.Resposta?.Conteudo ?? Enumerable.Empty<ExternalEmpenhoData>();
    }
    
    public async Task<IEnumerable<ExternalContratoData>> GetContratosAsync(int ano)
    {
        var response = await _httpClient.GetAsync($"DadosAbertos/ContratosEstaduais?ano={ano}");
        if (!response.IsSuccessStatusCode) return Enumerable.Empty<ExternalContratoData>();
        var json = await response.Content.ReadAsStringAsync();
        var tceResponse = JsonSerializer.Deserialize<TceWrapper<ExternalContratoData>>(json, _jsonOptions);
        return tceResponse?.Resposta?.Conteudo ?? Enumerable.Empty<ExternalContratoData>();
    }
    
    public async Task<IEnumerable<ExternalReceitaData>> GetReceitasAsync(int ano)
    {
        var response = await _httpClient.GetAsync($"DadosAbertos/ReceitasEstaduais?ano={ano}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tceResponse = JsonSerializer.Deserialize<TceWrapper<ExternalReceitaData>>(json, _jsonOptions);

        return tceResponse?.Resposta?.Conteudo ?? Enumerable.Empty<ExternalReceitaData>();
    }

    public async Task<IEnumerable<ExternalOrcamentoData>> GetOrcamentoAsync(int ano)
    {
        var response = await _httpClient.GetAsync($"DadosAbertos/OrcamentoEstadual?ano={ano}");
        if (!response.IsSuccessStatusCode) return Enumerable.Empty<ExternalOrcamentoData>();
        var json = await response.Content.ReadAsStringAsync();
        var tceResponse = JsonSerializer.Deserialize<TceWrapper<ExternalOrcamentoData>>(json, _jsonOptions);
        return tceResponse?.Resposta?.Conteudo ?? Enumerable.Empty<ExternalOrcamentoData>();
    }

    public async Task<int> GetTotalServidoresAsync(string codigoOrgao)
    {
        return 0; // TCE API mock simplificado para servidores
    }
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
