namespace TransparenciaPE.Domain.Interfaces;

/// <summary>
/// Abstraction for the external Pernambuco Transparency Portal API.
/// </summary>
public interface IPEDataClient
{
    Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosAsync(int ano);
    Task<IEnumerable<ExternalContratoData>> GetContratosAsync(int ano);
}

/// <summary>
/// Raw data from external API - Empenho.
/// </summary>
public class ExternalEmpenhoData
{
    public string NumeroEmpenho { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string NomeOrgao { get; set; } = string.Empty;
    public string CodigoOrgao { get; set; } = string.Empty;
    public string SiglaOrgao { get; set; } = string.Empty;
    public string Credor { get; set; } = string.Empty;
    public string CnpjCredor { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataEmpenho { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
    public string Subfuncao { get; set; } = string.Empty;
    public string NaturezaDespesa { get; set; } = string.Empty;
}

/// <summary>
/// Raw data from external API - Contrato.
/// </summary>
public class ExternalContratoData
{
    public string NumeroContrato { get; set; } = string.Empty;
    public string NomeOrgao { get; set; } = string.Empty;
    public string CodigoOrgao { get; set; } = string.Empty;
    public string Fornecedor { get; set; } = string.Empty;
    public string CnpjFornecedor { get; set; } = string.Empty;
    public decimal ValorContrato { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string Objeto { get; set; } = string.Empty;
}
