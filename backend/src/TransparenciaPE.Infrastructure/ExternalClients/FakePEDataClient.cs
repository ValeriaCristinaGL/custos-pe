using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.Infrastructure.ExternalClients;

/// <summary>
/// Mock implementation of the PE Transparency Portal API for development and testing.
/// Replace with real Refit/HttpClient implementation when the external API is available.
/// </summary>
public class FakePEDataClient : IPEDataClient
{
    public Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosAsync(int ano)
    {
        var data = new List<ExternalEmpenhoData>
        {
            new()
            {
                NumeroEmpenho = $"EMP-{ano}-001",
                Ano = ano,
                NomeOrgao = "Secretaria de Educação e Esportes",
                CodigoOrgao = "SEE",
                SiglaOrgao = "SEE",
                Credor = "Editora Brasil Ltda",
                CnpjCredor = "11.222.333/0001-81",
                Valor = 150_000.00m,
                DataEmpenho = new DateTime(ano, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Descricao = "Aquisição de livros didáticos",
                Funcao = "12",
                Subfuncao = "361",
                NaturezaDespesa = "3.3.90.30"
            },
            new()
            {
                NumeroEmpenho = $"EMP-{ano}-002",
                Ano = ano,
                NomeOrgao = "Secretaria de Saúde",
                CodigoOrgao = "SES",
                SiglaOrgao = "SES",
                Credor = "MedEquip Comércio de Equipamentos",
                CnpjCredor = "44.555.666/0001-99",
                Valor = 500_000.00m,
                DataEmpenho = new DateTime(ano, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                Descricao = "Compra de equipamentos hospitalares",
                Funcao = "10",
                Subfuncao = "302",
                NaturezaDespesa = "4.4.90.52"
            },
            new()
            {
                NumeroEmpenho = $"EMP-{ano}-003",
                Ano = ano,
                NomeOrgao = "Secretaria de Infraestrutura e Recursos Hídricos",
                CodigoOrgao = "SEINFRA",
                SiglaOrgao = "SEINFRA",
                Credor = "Construtora Recife S.A.",
                CnpjCredor = "77.888.999/0001-00",
                Valor = 2_000_000.00m,
                DataEmpenho = new DateTime(ano, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                Descricao = "Obras de pavimentação",
                Funcao = "15",
                Subfuncao = "451",
                NaturezaDespesa = "4.4.90.51"
            }
        };

        return Task.FromResult<IEnumerable<ExternalEmpenhoData>>(data);
    }

    public Task<IEnumerable<ExternalContratoData>> GetContratosAsync(int ano)
    {
        var data = new List<ExternalContratoData>
        {
            new()
            {
                NumeroContrato = $"CT-{ano}-001",
                NomeOrgao = "Secretaria de Educação e Esportes",
                CodigoOrgao = "SEE",
                Fornecedor = "Editora Brasil Ltda",
                CnpjFornecedor = "11.222.333/0001-81",
                ValorContrato = 450_000.00m,
                DataInicio = new DateTime(ano, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new DateTime(ano, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                Objeto = "Fornecimento de material didático para rede estadual"
            },
            new()
            {
                NumeroContrato = $"CT-{ano}-002",
                NomeOrgao = "Secretaria de Saúde",
                CodigoOrgao = "SES",
                Fornecedor = "MedEquip Comércio de Equipamentos",
                CnpjFornecedor = "44.555.666/0001-99",
                ValorContrato = 1_200_000.00m,
                DataInicio = new DateTime(ano, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new DateTime(ano + 1, 2, 28, 0, 0, 0, DateTimeKind.Utc),
                Objeto = "Manutenção e fornecimento de equipamentos médicos"
            }
        };

        return Task.FromResult<IEnumerable<ExternalContratoData>>(data);
    }
}
