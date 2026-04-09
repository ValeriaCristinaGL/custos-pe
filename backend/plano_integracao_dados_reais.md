# Plano de Integração — Dados Reais do TCE-PE (Tribunal de Contas do Estado de Pernambuco)

## Contexto

A aplicação **TransparenciaPE** já está funcionando com dados fake (`FakePEDataClient`). Agora precisamos substituir pelo client real que consome a **API de Dados Abertos do TCE-PE**.

### Arquitetura Atual (não alterar)
```
d:\Projetinho\
├── src/
│   ├── TransparenciaPE.Domain/         ← Entidades e interfaces
│   ├── TransparenciaPE.Application/    ← Services, Helpers, DTOs
│   ├── TransparenciaPE.Infrastructure/ ← Repositories, DbContext, Clients
│   └── TransparenciaPE.API/            ← Controllers, Program.cs
└── tests/
    └── TransparenciaPE.UnitTests/      ← xUnit + Moq
```

A interface que o client real deve implementar já existe:
- **Arquivo**: `src/TransparenciaPE.Domain/Interfaces/IPEDataClient.cs`
- **Interface**: `IPEDataClient` com métodos `GetEmpenhosAsync(int ano)` e `GetContratosAsync(int ano)`
- **DTOs de retorno**: `ExternalEmpenhoData` e `ExternalContratoData` (no mesmo arquivo)

### O que precisa ser feito
Criar um `TcePEDataClient` em `src/TransparenciaPE.Infrastructure/ExternalClients/` que implementa `IPEDataClient` consumindo a API real do TCE-PE.

---

## Fonte de Dados: API de Dados Abertos do TCE-PE

### URL Base
```
https://sistemas.tce.pe.gov.br/DadosAbertos/
```

### Padrão de URL
```
https://sistemas.tce.pe.gov.br/DadosAbertos/{Metodo}!{formato}?{param1}={valor1}&{param2}={valor2}
```

### Formatos suportados: `json`, `xml`, `html`

### Limitação importante: A API retorna no máximo **10.000 registros** por chamada.

---

## Endpoints que devemos consumir

### 1. DespesasEstaduais (Empenhos/Despesas)
```
GET https://sistemas.tce.pe.gov.br/DadosAbertos/DespesasEstaduais!json?ANO_DESPESA={ano}
```

**Campos de saída esperados** (verificar na resposta real e ajustar):
| Campo API (provável) | Mapear para `ExternalEmpenhoData` |
|---|---|
| `NUMERO_EMPENHO` ou similar | `NumeroEmpenho` |
| `ANO_DESPESA` | `Ano` |
| `NOME_UNIDADE_GESTORA` ou `DESCRICAO_UG` | `NomeOrgao` |
| `CODIGO_UNIDADE_GESTORA` | `CodigoOrgao` |
| `NOME_CREDOR` ou `CREDOR` | `Credor` |
| `CNPJ_CPF_CREDOR` ou `CPF_CNPJ` | `CnpjCredor` |
| `VALOR_EMPENHADO` ou `VALOR` | `Valor` |
| `DATA_EMPENHO` ou `DATA_DESPESA` | `DataEmpenho` |
| `DESCRICAO` ou `HISTORICO` | `Descricao` |
| `FUNCAO` | `Funcao` |
| `SUBFUNCAO` | `Subfuncao` |
| `NATUREZA_DESPESA` ou `ELEMENTO_DESPESA` | `NaturezaDespesa` |

> **IMPORTANTE**: Os nomes dos campos acima são estimativas. Ao implementar, faça uma chamada real à API primeiro para ver os nomes exatos dos campos JSON retornados e ajuste o mapeamento.

### 2. Contratos
```
GET https://sistemas.tce.pe.gov.br/DadosAbertos/Contratos!json?ANO_CONTRATO={ano}
```

**Campos de saída esperados** (verificar na resposta real e ajustar):
| Campo API (provável) | Mapear para `ExternalContratoData` |
|---|---|
| `NUMERO_CONTRATO` | `NumeroContrato` |
| `NOME_UNIDADE_GESTORA` | `NomeOrgao` |
| `CODIGO_UNIDADE_GESTORA` | `CodigoOrgao` |
| `NOME_CONTRATADO` ou `CONTRATADO` | `Fornecedor` |
| `CNPJ_CPF_CONTRATADO` | `CnpjFornecedor` |
| `VALOR_CONTRATO` | `ValorContrato` |
| `DATA_INICIO` ou `DATA_ASSINATURA` | `DataInicio` |
| `DATA_FIM` ou `DATA_TERMINO` | `DataFim` |
| `OBJETO` ou `DESCRICAO_OBJETO` | `Objeto` |

### 3. EmpenhoResumo (complementar, se necessário)
```
GET https://sistemas.tce.pe.gov.br/DadosAbertos/EmpenhoResumo!json?ANO_REFERENCIA={ano}
```

### 4. EmpenhoLiquidacao (para dados de liquidação)
```
GET https://sistemas.tce.pe.gov.br/DadosAbertos/EmpenhoLiquidacao!json?ANO_REFERENCIA={ano}
```

### 5. EmpenhoPagamento (para dados de pagamento)
```
GET https://sistemas.tce.pe.gov.br/DadosAbertos/EmpenhoPagamento!json?ANO_REFERENCIA={ano}
```

### 6. UnidadesJurisdicionadasEstaduais (para listar órgãos)
```
GET https://sistemas.tce.pe.gov.br/DadosAbertos/UnidadesJurisdicionadasEstaduais!json
```

---

## Tarefas para o OpenCode

### Tarefa 1 — Descobrir os campos reais da API

Antes de codar, preciso que você descubra a estrutura real da resposta JSON. Execute chamadas HTTP (pode usar curl, PowerShell ou Invoke-WebRequest) para:

```powershell
# Despesas Estaduais de 2025
Invoke-WebRequest -Uri "https://sistemas.tce.pe.gov.br/DadosAbertos/DespesasEstaduais!json?ANO_DESPESA=2025" -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json | Select-Object -First 1

# Contratos de 2025
Invoke-WebRequest -Uri "https://sistemas.tce.pe.gov.br/DadosAbertos/Contratos!json?ANO_CONTRATO=2025" -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json | Select-Object -First 1

# Unidades Gestoras Estaduais
Invoke-WebRequest -Uri "https://sistemas.tce.pe.gov.br/DadosAbertos/UnidadesJurisdicionadasEstaduais!json" -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json | Select-Object -First 1
```

Analise os nomes dos campos retornados e documente-os em um comentário no código.

### Tarefa 2 — Criar os DTOs de resposta da API do TCE-PE

Crie um arquivo em:
```
src/TransparenciaPE.Infrastructure/ExternalClients/TceModels/
```

Crie classes C# que mapeiam EXATAMENTE o JSON retornado pela API do TCE-PE.
- Use `[JsonPropertyName("NOME_DO_CAMPO")]` para mapear os nomes snake_case/uppercase da API para propriedades C# normais.
- Exemplo:
```csharp
using System.Text.Json.Serialization;

public class TceDespesaEstadual
{
    [JsonPropertyName("ANO_DESPESA")]
    public int AnoDespesa { get; set; }

    [JsonPropertyName("NUMERO_EMPENHO")]
    public string? NumeroEmpenho { get; set; }

    // ... etc, baseado no que a Tarefa 1 retornou
}
```

### Tarefa 3 — Criar o `TcePEDataClient`

Crie o arquivo:
```
src/TransparenciaPE.Infrastructure/ExternalClients/TcePEDataClient.cs
```

Requisitos:
1. Implementa `IPEDataClient` (namespace `TransparenciaPE.Domain.Interfaces`)
2. Usa `HttpClient` via `IHttpClientFactory` injetado
3. Consulta a API do TCE-PE e desserializa o JSON
4. Mapeia os DTOs do TCE (`TceDespesaEstadual`, `TceContrato`) para os DTOs do domínio (`ExternalEmpenhoData`, `ExternalContratoData`)
5. Lidar com o limite de 10.000 registros (paginação se necessário)
6. Retry com Polly ou loop de tentativas em caso de falha
7. Logging com `ILogger<TcePEDataClient>`
8. **Todas as datas devem ser UTC** (`DateTime.SpecifyKind(..., DateTimeKind.Utc)`)

Exemplo de estrutura:
```csharp
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.Infrastructure.ExternalClients;

public class TcePEDataClient : IPEDataClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TcePEDataClient> _logger;
    private const string BaseUrl = "https://sistemas.tce.pe.gov.br/DadosAbertos";

    public TcePEDataClient(HttpClient httpClient, ILogger<TcePEDataClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosAsync(int ano)
    {
        var url = $"{BaseUrl}/DespesasEstaduais!json?ANO_DESPESA={ano}";
        _logger.LogInformation("Fetching despesas from TCE-PE for year {Ano}", ano);

        var response = await _httpClient.GetFromJsonAsync<List<TceDespesaEstadual>>(url);
        
        // Mapear TceDespesaEstadual → ExternalEmpenhoData
        return response?.Select(d => new ExternalEmpenhoData
        {
            NumeroEmpenho = d.NumeroEmpenho ?? "",
            Ano = d.AnoDespesa,
            // ... mapear todos os campos
        }) ?? Enumerable.Empty<ExternalEmpenhoData>();
    }

    public async Task<IEnumerable<ExternalContratoData>> GetContratosAsync(int ano)
    {
        var url = $"{BaseUrl}/Contratos!json?ANO_CONTRATO={ano}";
        // ... mesma lógica
    }
}
```

### Tarefa 4 — Registrar o client real no DI

No arquivo `src/TransparenciaPE.API/Extensions/ServiceCollectionExtensions.cs`, no método `AddInfrastructureServices`:

1. Adicione o `HttpClient` com `IHttpClientFactory`:
```csharp
services.AddHttpClient<IPEDataClient, TcePEDataClient>(client =>
{
    client.BaseAddress = new Uri("https://sistemas.tce.pe.gov.br/DadosAbertos/");
    client.Timeout = TimeSpan.FromSeconds(60); // API pode ser lenta
});
```

2. **Remova** a linha:
```csharp
services.AddScoped<IPEDataClient, FakePEDataClient>();
```

3. Não delete o arquivo `FakePEDataClient.cs` — mantenha como referência.

### Tarefa 5 — Testes unitários (TDD)

Crie testes em:
```
tests/TransparenciaPE.UnitTests/ExternalClients/TcePEDataClientTests.cs
```

Teste:
1. Mapeamento correto dos campos do TCE para `ExternalEmpenhoData`
2. CNPJ é preservado (sanitização ocorre no `DataSyncService`)
3. Datas são convertidas para UTC
4. Tratamento de resposta vazia da API
5. Tratamento de erro HTTP (timeout, 500, etc.)

Use `Moq` para mockar o `HttpMessageHandler`:
```csharp
var mockHandler = new Mock<HttpMessageHandler>();
mockHandler.Protected()
    .Setup<Task<HttpResponseMessage>>("SendAsync", 
        ItExpr.IsAny<HttpRequestMessage>(), 
        ItExpr.IsAny<CancellationToken>())
    .ReturnsAsync(new HttpResponseMessage
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(jsonResponse)
    });
```

### Tarefa 6 — Verificação

1. Rode `dotnet test` — todos os testes devem passar (os 46 existentes + os novos)
2. Rode `dotnet build` — zero erros
3. Rode a API: `dotnet run --project src/TransparenciaPE.API --urls "http://localhost:5000"`
4. Verifique nos logs se o Background Worker sincronizou dados reais
5. Acesse `http://localhost:5000/swagger` e teste `/api/v1/dashboard/resumo` para ver KPIs reais
6. Commit: `feat: integrate real TCE-PE open data API for empenhos and contratos`

### Tarefa 7 — Commit

```bash
git add -A
git commit -m "feat: integrate real TCE-PE open data API for empenhos and contratos"
```

Use `nathannmvr@gmail.com` como e-mail do autor (já configurado no repositório).

---

## Observações Importantes

1. **Limite de 10.000 registros**: A API do TCE-PE retorna no máximo 10k registros por chamada. Se houver mais dados, pode ser necessário paginar por Unidade Gestora ou mês.

2. **Performance**: A API do TCE-PE pode ser lenta (10-30 segundos por chamada). Use `HttpClient.Timeout` de pelo menos 60 segundos.

3. **Sem autenticação**: A API é pública, não precisa de token/chave.

4. **Formato de datas**: Verifique o formato das datas no JSON (pode ser `"dd/MM/yyyy"` ou `"yyyy-MM-dd"`). Use `JsonSerializerOptions` com custom converter se necessário.

5. **Encoding**: Respostas podem vir em ISO-8859-1 ao invés de UTF-8. Se caracteres acentuados ficarem errados, trate o encoding.

6. **Código existente não deve ser quebrado**: Os 46 testes unitários existentes devem continuar passando.

7. **Padrão do projeto**: Use `ILogger`, injeção de dependência, e siga o padrão SOLID que já existe no projeto. Leia o `src/TransparenciaPE.Infrastructure/ExternalClients/FakePEDataClient.cs` como referência da interface que deve ser implementada.
