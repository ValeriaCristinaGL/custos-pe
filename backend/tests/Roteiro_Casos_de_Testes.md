# Roteiro de Casos de Testes - TransparenciaPE Backend

<style>
@page {
  size: A4 landscape;
  margin: 10mm;
}

body {
  font-family: Arial, sans-serif;
  font-size: 10px;
  line-height: 1.35;
}

h1 {
  font-size: 24px;
}

h2 {
  font-size: 18px;
  page-break-before: auto;
}

h3 {
  font-size: 14px;
}

h4 {
  font-size: 12px;
  margin: 12px 0 4px;
}

table {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed;
  margin: 8px 0 14px;
}

th,
td {
  border: 1px solid #d0d7de;
  padding: 4px 6px;
  vertical-align: top;
  overflow-wrap: anywhere;
}

th {
  background: #f6f8fa;
}

thead {
  display: table-header-group;
}

tr {
  page-break-inside: avoid;
}

ul {
  margin: 4px 0 10px;
  padding-left: 18px;
}

ol {
  margin: 3px 0 4px;
  padding-left: 18px;
}

li {
  margin: 2px 0;
}

code {
  white-space: normal;
  overflow-wrap: anywhere;
}

pre code {
  white-space: pre-wrap;
}
</style>

- **Projeto:** TransparenciaPE - API de Dados Fiscais do Estado de Pernambuco
- **Disciplina:** Verificacao e Validacao de Software
- **Tipos de Teste:** Testes unitarios e testes de integracao
- **Frameworks:** xUnit + Moq + FluentAssertions + Microsoft.AspNetCore.Mvc.Testing
- **Cobertura Alvo:** Application, Domain, Infrastructure e API
- **Data:** 19/06/2026

---

## Como executar

Executar toda a suite:

```bash
dotnet test
```

Executar apenas testes unitarios:

```bash
dotnet test tests/TransparenciaPE.UnitTests/TransparenciaPE.UnitTests.csproj
```

Executar apenas testes de integracao:

```bash
dotnet test tests/TransparenciaPE.IntegrationTests/TransparenciaPE.IntegrationTests.csproj
```

Executar por metodo:

```bash
dotnet test --filter "GetResumoAsync_ReturnsKPIs_WhenDataExists"
```

Executar por classe:

```bash
dotnet test --filter "FullyQualifiedName~DashboardServiceTests"
```

---

## Convencoes

Os metodos de teste seguem o padrao:

```text
MetodoOuAssunto_ComportamentoEsperado_WhenCondicao
```

Tambem sao aceitas variantes com `For`, `With`, `By` e `On` quando a condicao fica mais natural.

Exemplos:

- `GetResumoAsync_ReturnsKPIs_WhenDataExists`
- `ExportarCsvAsync_UsesYearFilter_WhenYearIsProvided`
- `GetEmpenhosAsync_ReturnsSeedData_ForRequestedYear`

Os projetos possuem testes de convencao que falham caso um metodo `[Fact]` ou `[Theory]` saia desse padrao.

---

## Sumario

> **CTs (metodos)** correspondem aos metodos de teste declarados no codigo.
>
> **Execucoes xUnit** correspondem ao total executado pelo xUnit; metodos `[Theory]` com multiplos dados executam mais de uma vez.

| Suite       | CTs (metodos) | Execucoes xUnit |
| ----------- | :-----------: | :-------------: |
| Unitarios   |      104      |       126       |
| Integracao  |       12      |       12        |
| **Total**   |    **116**    |     **138**     |

### Sumario por modulo - UnitTests

| Modulo                                   | CTs | Execucoes | Observacao                         |
| ---------------------------------------- | :---: | :-------: | ---------------------------------- |
| Helpers - CnpjHelper                     |  6  |    15     | 4 theories com `InlineData`        |
| Helpers - McaspMapper                    |  6  |    14     | 5 theories com `InlineData`        |
| Entities - Orcamento                     |  3  |     3     | -                                  |
| Entities - OrgaoGoverno                  |  5  |     5     | -                                  |
| Entities - Receita                       |  3  |     3     | -                                  |
| Services - DashboardService              |  7  |     7     | -                                  |
| Services - PesquisaService               |  6  |     9     | 2 theories com `InlineData`        |
| Services - DataSyncService               |  8  |     8     | -                                  |
| Controllers - DashboardController        | 13  |    13     | -                                  |
| Controllers - PesquisaController         |  8  |     8     | -                                  |
| Infrastructure - AppDbContext            |  7  |     7     | -                                  |
| Infrastructure - FakePEDataClient        |  6  |     6     | -                                  |
| Infrastructure - Repository              | 12  |    12     | -                                  |
| Infrastructure - UnitOfWork              |  3  |     3     | -                                  |
| ExternalClients - TcePEDataClient        |  7  |     7     | -                                  |
| Middlewares - GlobalExceptionMiddleware  |  3  |     5     | 1 theory com `MemberData`          |
| Conventions - TestNamingConvention       |  1  |     1     | Valida nomes dos testes unitarios  |
| **Total**                                | 104 |    126    | -                                  |

### Sumario por modulo - IntegrationTests

| Modulo                             | CTs | Execucoes | Observacao                           |
| ---------------------------------- | :---: | :-------: | ------------------------------------ |
| ApiEndpoints                       | 11  |    11     | Contratos HTTP usando `TestServer`   |
| Conventions - TestNamingConvention |  1  |     1     | Valida nomes dos testes de integracao |
| **Total**                          | 12  |    12     | -                                    |

---

## Casos de teste unitarios

### Helpers - CnpjHelper

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-001 | `Sanitize_ReturnsDigitsOnly_WhenInputHasPunctuation` | Remove pontuacao e retorna apenas digitos. |
| CT-002 | `Sanitize_ReturnsSameValue_WhenInputHasOnlyDigits` | Mantem CNPJ ja sanitizado. |
| CT-003 | `Sanitize_ReturnsDigitsOnly_WhenInputHasSurroundingWhitespace` | Remove espacos externos e pontuacao. |
| CT-004 | `Sanitize_ReturnsEmpty_WhenInputIsNullOrWhitespace` | Retorna vazio para `null`, vazio ou branco. |
| CT-005 | `IsValid_ReturnsTrue_WhenCnpjIsValid` | Aceita CNPJs validos com ou sem mascara. |
| CT-006 | `IsValid_ReturnsFalse_WhenCnpjIsInvalid` | Rejeita CNPJs invalidos, vazios, nulos ou com tamanho incorreto. |

### Helpers - McaspMapper

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-007 | `MapToClassificacao_ReturnsPessoalEncargos_WhenNaturezaStartsWith31` | Mapeia prefixo `3.1` para pessoal e encargos sociais. |
| CT-008 | `MapToClassificacao_ReturnsCusteio_WhenNaturezaStartsWith33` | Mapeia prefixo `3.3` para custeio. |
| CT-009 | `MapToClassificacao_ReturnsInvestimentos_WhenNaturezaStartsWith44` | Mapeia prefixo `4.4` para investimentos. |
| CT-010 | `MapToClassificacao_ReturnsOutros_WhenNaturezaPrefixIsUnknown` | Retorna outros para prefixos nao mapeados. |
| CT-011 | `MapToClassificacao_ReturnsOutros_WhenNaturezaIsBlank` | Retorna outros para natureza vazia ou em branco. |
| CT-012 | `MapToClassificacao_ReturnsOutros_WhenNaturezaIsNull` | Retorna outros para natureza nula. |

### Entities - Orcamento

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-013 | `Orcamento_AssignsBudgetFields_WhenInitialized` | Atribui ano, dotacao inicial e dotacao atualizada. |
| CT-014 | `Orcamento_AssignsOrgaoGovernoId_WhenInitialized` | Atribui o identificador do orgao associado. |
| CT-015 | `Orcamento_GeneratesId_WhenCreated` | Gera `Id` automaticamente pela entidade base. |

### Entities - OrgaoGoverno

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-016 | `OrgaoGoverno_GeneratesUniqueId_WhenCreated` | Gera IDs nao vazios e distintos. |
| CT-017 | `OrgaoGoverno_InitializesCollections_WhenCreated` | Inicializa colecoes de empenhos e contratos vazias. |
| CT-018 | `OrgaoGoverno_SetsCreatedAt_WhenCreated` | Define `CreatedAt` no momento da criacao e mantem `UpdatedAt` nulo. |
| CT-019 | `OrgaoGoverno_AssignsIdentificationFields_WhenInitialized` | Atribui codigo, nome, sigla e tipo. |
| CT-020 | `OrgaoGoverno_AssignsCapacityFields_WhenInitialized` | Atribui total de servidores e orcamento atual. |

### Entities - Receita

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-021 | `Receita_AssignsAccountingFields_WhenInitialized` | Atribui valor, mes, ano e origem. |
| CT-022 | `Receita_AssignsOrgaoGovernoId_WhenInitialized` | Atribui o identificador do orgao associado. |
| CT-023 | `Receita_GeneratesId_WhenCreated` | Gera `Id` automaticamente pela entidade base. |

### Services - DashboardService

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-024 | `GetResumoAsync_ReturnsKPIs_WhenDataExists` | Retorna KPIs e calcula percentual executado quando ha dados. |
| CT-025 | `GetResumoAsync_ReturnsZeroPercentualExecutado_WhenTotalEmpenhadoIsZero` | Evita divisao por zero quando total empenhado e zero. |
| CT-026 | `GetResumoAsync_PassesYearParameter_WhenYearIsProvided` | Encaminha o filtro de ano ao servico de query. |
| CT-027 | `GetComparativoOrgaosAsync_ReturnsAno_WhenYearIsProvided` | Retorna o ano solicitado no DTO de comparativo. |
| CT-028 | `GetComparativoOrgaosAsync_ReturnsOrgaos_WhenDataExists` | Mapeia lista de orgaos retornada pela query. |
| CT-029 | `GetDrillDownAsync_ReturnsCodigoOrgao_WhenCodigoOrgaoIsProvided` | Retorna o codigo do orgao solicitado. |
| CT-030 | `GetDrillDownAsync_ReturnsItens_WhenDataExists` | Mapeia itens de drill-down retornados pela query. |

### Services - PesquisaService

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-031 | `PesquisaGlobalAsync_SearchesContratosByCnpj_WhenTermIsCnpj` | Sanitiza termo CNPJ e pesquisa contratos por CNPJ. |
| CT-032 | `PesquisaGlobalAsync_SearchesContratosByFornecedor_WhenTermIsText` | Pesquisa contratos por fornecedor quando termo e texto. |
| CT-033 | `PesquisaGlobalAsync_ThrowsArgumentException_WhenTermIsEmpty` | Rejeita termo nulo, vazio ou em branco. |
| CT-034 | `ExportarCsvAsync_ReturnsCsvWithEmpenhoData_WhenDataExists` | Gera CSV com dados de empenho. |
| CT-035 | `ExportarCsvAsync_ReturnsHeaderOnly_WhenNoDataExists` | Gera apenas cabecalho quando nao ha dados. |
| CT-036 | `ExportarCsvAsync_UsesYearFilter_WhenYearIsProvided` | Aplica filtro de ano no predicado enviado ao repositorio. |

### Services - DataSyncService

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-037 | `SyncEmpenhosAsync_AddsEmpenho_WhenEmpenhoDoesNotExist` | Insere empenho novo retornado pela API externa. |
| CT-038 | `SyncEmpenhosAsync_UpdatesEmpenho_WhenEmpenhoExists` | Atualiza empenho existente sem inserir duplicado. |
| CT-039 | `SyncEmpenhosAsync_SanitizesCnpj_WhenAddingEmpenho` | Salva CNPJ do credor apenas com digitos. |
| CT-040 | `SyncAllAsync_ReturnsSyncedAt_WhenSyncCompletes` | Retorna timestamp de sincronizacao ao concluir. |
| CT-041 | `SyncEmpenhosAsync_ReturnsZero_WhenNoOrgaosExist` | Nao consulta API externa quando nao ha orgaos base. |
| CT-042 | `SyncEmpenhosAsync_ReturnsZero_WhenApiReturnsEmpty` | Retorna zero quando a API nao traz empenhos. |
| CT-043 | `SyncContratosAsync_AddsContrato_WhenContratoDoesNotExist` | Insere contrato novo e sanitiza CNPJ do fornecedor. |
| CT-044 | `SyncAllAsync_ReturnsProcessedCounts_WhenEmpenhosAndContratosExist` | Agrega contadores de empenhos e contratos processados. |

### Controllers - DashboardController

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-045 | `GetResumo_ReturnsOkResult_WhenServiceReturnsResumo` | Retorna `OkObjectResult` no resumo. |
| CT-046 | `GetResumo_ReturnsDashboardResumoDto_WhenServiceReturnsResumo` | Retorna `DashboardResumoDto` no corpo. |
| CT-047 | `GetResumo_ReturnsTotalEmpenhado_WhenServiceReturnsResumo` | Propaga `TotalEmpenhado` retornado pelo servico. |
| CT-048 | `GetResumo_InvokesServiceWithNullYear_WhenAnoIsNull` | Chama o servico com ano nulo. |
| CT-049 | `GetResumo_InvokesServiceWithProvidedYear_WhenAnoIsProvided` | Chama o servico com ano informado. |
| CT-050 | `GetComparativo_ReturnsOkResult_WhenServiceReturnsComparativo` | Retorna `OkObjectResult` no comparativo. |
| CT-051 | `GetComparativo_ReturnsComparativoOrgaosDto_WhenServiceReturnsComparativo` | Retorna `ComparativoOrgaosDto` no corpo. |
| CT-052 | `GetComparativo_ReturnsAno_WhenServiceReturnsComparativo` | Propaga ano retornado pelo servico. |
| CT-053 | `GetEvolucao_ReturnsOkResult_WhenServiceReturnsDrillDown` | Retorna `OkObjectResult` na evolucao. |
| CT-054 | `GetEvolucao_ReturnsDrillDownDto_WhenServiceReturnsDrillDown` | Retorna `DrillDownDto` no corpo. |
| CT-055 | `GetEvolucao_ReturnsCodigoOrgao_WhenServiceReturnsDrillDown` | Propaga codigo do orgao retornado pelo servico. |
| CT-056 | `GetEvolucao_InvokesServiceWithNullYear_WhenAnoIsNull` | Chama o servico com codigo e ano nulo. |
| CT-057 | `GetEvolucao_InvokesServiceWithProvidedParameters_WhenAnoIsProvided` | Chama o servico com codigo e ano informado. |

### Controllers - PesquisaController

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-058 | `PesquisaGlobal_ReturnsOkResult_WhenServiceReturnsResult` | Retorna `OkObjectResult` na pesquisa. |
| CT-059 | `PesquisaGlobal_ReturnsPesquisaResultDto_WhenServiceReturnsResult` | Retorna `PesquisaResultDto` no corpo. |
| CT-060 | `PesquisaGlobal_ReturnsTermoBuscado_WhenServiceReturnsResult` | Propaga termo buscado retornado pelo servico. |
| CT-061 | `PesquisaGlobal_ReturnsBadRequestStatusCode_WhenTermoIsEmpty` | Retorna status 400 quando termo e vazio. |
| CT-062 | `PesquisaGlobal_ReturnsBadRequestObjectResult_WhenTermoIsEmpty` | Retorna `BadRequestObjectResult` quando termo e vazio. |
| CT-063 | `ExportarCsv_ReturnsFileContentResult_WhenServiceReturnsCsv` | Retorna arquivo quando servico gera CSV. |
| CT-064 | `ExportarCsv_ReturnsCsvContentType_WhenServiceReturnsCsv` | Define `ContentType` como `text/csv`. |
| CT-065 | `ExportarCsv_ReturnsFileNameWithCsvExtension_WhenServiceReturnsCsv` | Define nome de arquivo com extensao `.csv`. |

### Infrastructure - AppDbContext

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-066 | `Model_AppliesEmpenhoConfiguration_WhenBuilt` | Aplica tabela, tamanhos e tipo decimal de empenhos. |
| CT-067 | `Model_AppliesContratoConfiguration_WhenBuilt` | Aplica tabela e limites de contrato. |
| CT-068 | `Model_AppliesOrgaoGovernoConfiguration_WhenBuilt` | Aplica tabela e limites de orgao. |
| CT-069 | `Model_AppliesLiquidacaoConfiguration_WhenBuilt` | Aplica tabela e limite de liquidacao. |
| CT-070 | `Model_AppliesPagamentoConfiguration_WhenBuilt` | Aplica tabela e limite de pagamento. |
| CT-071 | `SaveChangesAsync_SetsCreatedAt_WhenEntityIsAdded` | Atualiza `CreatedAt` ao adicionar entidade. |
| CT-072 | `SaveChangesAsync_SetsUpdatedAt_WhenEntityIsModified` | Atualiza `UpdatedAt` ao modificar entidade. |

### Infrastructure - FakePEDataClient

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-073 | `GetEmpenhosAsync_ReturnsSeedData_ForRequestedYear` | Retorna empenhos fake do ano solicitado. |
| CT-074 | `GetEmpenhosByOrgaoAsync_ReturnsFilteredData_ForRequestedOrgao` | Filtra empenhos fake por orgao. |
| CT-075 | `GetContratosAsync_ReturnsSeedData_ForRequestedYear` | Retorna contratos fake do ano solicitado. |
| CT-076 | `GetReceitasAsync_ReturnsSeedData_ForRequestedYear` | Retorna receitas fake do ano solicitado. |
| CT-077 | `GetOrcamentoAsync_ReturnsSeedData_ForRequestedYear` | Retorna orcamento fake do ano solicitado. |
| CT-078 | `GetTotalServidoresAsync_ReturnsFixedTotal_ForAnyOrgao` | Retorna total fixo de servidores. |

### Infrastructure - Repository

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-079 | `AddAsync_PersistsEntity_WhenSaved` | Persiste entidade adicionada. |
| CT-080 | `GetByIdAsync_ReturnsEntity_WhenEntityExists` | Busca entidade por ID. |
| CT-081 | `GetAllAsync_ReturnsEntities_WhenEntitiesExist` | Lista entidades existentes. |
| CT-082 | `FindAsync_ReturnsMatchingEntities_WhenPredicateMatches` | Filtra entidades por predicado. |
| CT-083 | `ExistsAsync_ReturnsTrue_WhenPredicateMatches` | Confirma existencia por predicado. |
| CT-084 | `Update_PersistsChanges_WhenSaved` | Persiste alteracoes de entidade. |
| CT-085 | `Remove_DeletesEntity_WhenSaved` | Remove entidade persistida. |
| CT-086 | `GetByNumeroAsync_ReturnsEmpenhoWithOrgao_WhenEmpenhoExists` | Busca empenho por numero/ano incluindo orgao. |
| CT-087 | `GetByAnoAsync_ReturnsOnlyMatchingEmpenhos_WhenYearIsProvided` | Filtra empenhos por ano. |
| CT-088 | `GetByOrgaoAsync_ReturnsOnlyMatchingEmpenhos_WhenOrgaoIsProvided` | Filtra empenhos por orgao. |
| CT-089 | `GetByNumeroAsync_ReturnsContratoWithOrgao_WhenContratoExists` | Busca contrato por numero incluindo orgao. |
| CT-090 | `SearchByCnpjAsync_ReturnsMatchingContratos_WhenCnpjExists` | Busca contratos por CNPJ. |

### Infrastructure - UnitOfWork

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-091 | `UnitOfWork_ExposesConfiguredRepositories_WhenCreated` | Expoe repositorios configurados. |
| CT-092 | `CommitAsync_PersistsPendingChanges_WhenRepositoryAddsEntity` | Persiste alteracoes pendentes. |
| CT-093 | `Dispose_DisposesDbContext_WhenCalled` | Descarta o contexto ao chamar `Dispose`. |

### ExternalClients - TcePEDataClient

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-094 | `GetReceitasAsync_ReturnsReceitas_WhenApiReturnsTceJson` | Desserializa receitas no envelope JSON do TCE. |
| CT-095 | `GetReceitasAsync_ThrowsHttpRequestException_WhenApiReturnsServerError` | Lanca excecao em erro HTTP para receitas. |
| CT-096 | `GetEmpenhosByOrgaoAsync_ReturnsEmpenhos_WhenApiReturnsTceJson` | Desserializa empenhos por orgao. |
| CT-097 | `GetEmpenhosByOrgaoAsync_ReturnsEmpty_WhenApiReturnsError` | Retorna lista vazia em erro HTTP de empenhos. |
| CT-098 | `GetContratosAsync_ReturnsContratos_WhenApiReturnsTceJson` | Desserializa contratos. |
| CT-099 | `GetContratosAsync_ReturnsEmpty_WhenApiReturnsError` | Retorna lista vazia em erro HTTP de contratos. |
| CT-100 | `GetOrcamentoAsync_ReturnsEmpty_WhenApiReturnsError` | Retorna lista vazia em erro HTTP de orcamento. |

### Middlewares - GlobalExceptionMiddleware

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-101 | `InvokeAsync_CallsNext_WhenNoExceptionOccurs` | Chama o proximo delegate quando nao ha excecao. |
| CT-102 | `InvokeAsync_ReturnsExpectedStatusCode_ForGivenException` | Mapeia excecoes para status HTTP esperados. |
| CT-103 | `InvokeAsync_ReturnsJsonError_WhenExceptionOccurs` | Retorna corpo JSON com campos obrigatorios. |

### Conventions

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-104 | `TestMethods_FollowNamingConvention_WhenDeclaredInUnitAssembly` | Garante padrao de nome nos testes unitarios. |

---

## Casos de teste de integracao

### API Endpoints

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-105 | `GetResumo_ReturnsOkStatus_WhenAnoIsProvided` | `GET /api/v1/dashboard/resumo` retorna HTTP 200. |
| CT-106 | `GetResumo_ReturnsJsonContentType_WhenAnoIsProvided` | Resposta de resumo retorna `application/json`. |
| CT-107 | `GetResumo_ReturnsDashboardResumoPayload_WhenAnoIsProvided` | Payload de resumo contem KPIs esperados. |
| CT-108 | `GetEvolucao_ReturnsOkStatus_WhenCodigoOrgaoIsProvided` | `GET /api/v1/dashboard/evolucao` retorna HTTP 200. |
| CT-109 | `GetEvolucao_ReturnsDrillDownPayload_WhenCodigoOrgaoIsProvided` | Payload de evolucao contem codigo do orgao e itens. |
| CT-110 | `PesquisaGlobal_ReturnsBadRequestStatus_WhenTermoIsEmpty` | Pesquisa global retorna HTTP 400 para termo vazio. |
| CT-111 | `PesquisaGlobal_ReturnsErrorPayload_WhenTermoIsEmpty` | Erro da pesquisa global retorna payload de erro. |
| CT-112 | `ExportarCsv_ReturnsOkStatus_WhenAnoIsProvided` | Exportacao CSV retorna HTTP 200. |
| CT-113 | `ExportarCsv_ReturnsCsvContentType_WhenAnoIsProvided` | Exportacao CSV retorna `text/csv`. |
| CT-114 | `ExportarCsv_ReturnsCsvHeader_WhenAnoIsProvided` | CSV inicia com cabecalho esperado. |
| CT-115 | `ExportarCsv_ReturnsCsvFileName_WhenAnoIsProvided` | Resposta CSV define nome de arquivo com `.csv`. |

### Conventions

| CT     | Metodo | Cenario validado |
| ------ | ------ | ---------------- |
| CT-116 | `TestMethods_FollowNamingConvention_WhenDeclaredInIntegrationAssembly` | Garante padrao de nome nos testes de integracao. |

---

## Detalhamento dos cenarios

### Modulo de Helpers - CnpjHelper

**Arquivo:** `tests/TransparenciaPE.UnitTests/Helpers/CnpjHelperTests.cs`

#### 1. Sanitize remove pontuacao quando a entrada contem mascara

- **ID do Caso de Teste:** CT-001
- **Nome:** Sanitize remove pontuacao quando a entrada contem mascara
- **Objetivo:** Validar o cenario: Sanitize remove pontuacao quando a entrada contem mascara.
- **Pre-condicoes:** Nenhuma; helper estatico sem dependencias externas.
- **Entradas:**
  1. `"11.222.333/0001-81"` e `"00.000.000/0000-00"`.
- **Passos para Execucao:**
  1. executar `CnpjHelper.Sanitize(input)` para cada entrada.
- **Resultado Esperado:** retorno contem apenas digitos, preservando a ordem numerica original.
- **Criterios de Sucesso:** retorno contem apenas digitos, preservando a ordem numerica original.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Sanitize preserva CNPJ que ja possui apenas digitos

- **ID do Caso de Teste:** CT-002
- **Nome:** Sanitize preserva CNPJ que ja possui apenas digitos
- **Objetivo:** Validar o cenario: Sanitize preserva CNPJ que ja possui apenas digitos.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. `"11222333000181"`.
- **Passos para Execucao:**
  1. chamar `CnpjHelper.Sanitize` com o valor ja sanitizado.
- **Resultado Esperado:** retorno exatamente igual a entrada, sem inclusao ou remocao indevida de digitos.
- **Criterios de Sucesso:** retorno exatamente igual a entrada, sem inclusao ou remocao indevida de digitos.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Sanitize remove espacos externos e pontuacao

- **ID do Caso de Teste:** CT-003
- **Nome:** Sanitize remove espacos externos e pontuacao
- **Objetivo:** Validar o cenario: Sanitize remove espacos externos e pontuacao.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. `"  11.222.333/0001-81  "`.
- **Passos para Execucao:**
  1. executar a sanitizacao sobre a string com espacos e mascara.
- **Resultado Esperado:** retorno `"11222333000181"`.
- **Criterios de Sucesso:** retorno `"11222333000181"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Sanitize retorna vazio para entrada nula, vazia ou em branco

- **ID do Caso de Teste:** CT-004
- **Nome:** Sanitize retorna vazio para entrada nula, vazia ou em branco
- **Objetivo:** Validar o cenario: Sanitize retorna vazio para entrada nula, vazia ou em branco.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. `null`, `""` e `"   "`.
- **Passos para Execucao:**
  1. chamar `CnpjHelper.Sanitize` para cada valor.
- **Resultado Esperado:** retorno `string.Empty`, sem lancar excecao.
- **Criterios de Sucesso:** retorno `string.Empty`, sem lancar excecao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. IsValid aceita CNPJs validos

- **ID do Caso de Teste:** CT-005
- **Nome:** IsValid aceita CNPJs validos
- **Objetivo:** Validar o cenario: IsValid aceita CNPJs validos.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. CNPJ valido com e sem mascara.
- **Passos para Execucao:**
  1. chamar `CnpjHelper.IsValid(cnpj)`.
- **Resultado Esperado:** retorno `true` para as duas representacoes do mesmo CNPJ valido.
- **Criterios de Sucesso:** retorno `true` para as duas representacoes do mesmo CNPJ valido.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. IsValid rejeita CNPJs invalidos

- **ID do Caso de Teste:** CT-006
- **Nome:** IsValid rejeita CNPJs invalidos
- **Objetivo:** Validar o cenario: IsValid rejeita CNPJs invalidos.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. CNPJs com todos os digitos iguais, tamanho incorreto, vazio, nulo e longo demais.
- **Passos para Execucao:**
  1. chamar `CnpjHelper.IsValid(cnpj)` para cada entrada.
- **Resultado Esperado:** retorno `false` em todos os casos invalidos.
- **Criterios de Sucesso:** retorno `false` em todos os casos invalidos.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Helpers - McaspMapper

**Arquivo:** `tests/TransparenciaPE.UnitTests/Helpers/McaspMapperTests.cs`

#### 1. Classificacao MCASP identifica despesas de pessoal

- **ID do Caso de Teste:** CT-007
- **Nome:** Classificacao MCASP identifica despesas de pessoal
- **Objetivo:** Validar o cenario: Classificacao MCASP identifica despesas de pessoal.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. naturezas iniciadas por `3.1`.
- **Passos para Execucao:**
  1. chamar `McaspMapper.MapToClassificacao(natureza, "")`.
- **Resultado Esperado:** retorno `"Pessoal e Encargos Sociais"`.
- **Criterios de Sucesso:** retorno `"Pessoal e Encargos Sociais"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Classificacao MCASP identifica custeio

- **ID do Caso de Teste:** CT-008
- **Nome:** Classificacao MCASP identifica custeio
- **Objetivo:** Validar o cenario: Classificacao MCASP identifica custeio.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. naturezas iniciadas por `3.3`.
- **Passos para Execucao:**
  1. mapear cada natureza informada.
- **Resultado Esperado:** retorno `"Custeio"`.
- **Criterios de Sucesso:** retorno `"Custeio"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Classificacao MCASP identifica investimentos

- **ID do Caso de Teste:** CT-009
- **Nome:** Classificacao MCASP identifica investimentos
- **Objetivo:** Validar o cenario: Classificacao MCASP identifica investimentos.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. naturezas iniciadas por `4.4`.
- **Passos para Execucao:**
  1. mapear cada natureza informada.
- **Resultado Esperado:** retorno `"Investimentos"`.
- **Criterios de Sucesso:** retorno `"Investimentos"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Classificacao MCASP trata prefixos desconhecidos como outros

- **ID do Caso de Teste:** CT-010
- **Nome:** Classificacao MCASP trata prefixos desconhecidos como outros
- **Objetivo:** Validar o cenario: Classificacao MCASP trata prefixos desconhecidos como outros.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. prefixos `3.2`, `4.5`, `4.6`, `5.0` e `10.0`.
- **Passos para Execucao:**
  1. chamar o mapper para cada prefixo nao mapeado.
- **Resultado Esperado:** retorno `"Outros"` para todos os casos.
- **Criterios de Sucesso:** retorno `"Outros"` para todos os casos.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Classificacao MCASP trata natureza em branco como outros

- **ID do Caso de Teste:** CT-011
- **Nome:** Classificacao MCASP trata natureza em branco como outros
- **Objetivo:** Validar o cenario: Classificacao MCASP trata natureza em branco como outros.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. `""` e `"   "`.
- **Passos para Execucao:**
  1. chamar o mapper com valor vazio ou apenas espacos.
- **Resultado Esperado:** retorno `"Outros"` sem erro de parsing.
- **Criterios de Sucesso:** retorno `"Outros"` sem erro de parsing.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Classificacao MCASP trata natureza nula como outros

- **ID do Caso de Teste:** CT-012
- **Nome:** Classificacao MCASP trata natureza nula como outros
- **Objetivo:** Validar o cenario: Classificacao MCASP trata natureza nula como outros.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. `null`.
- **Passos para Execucao:**
  1. chamar o mapper com `null!`.
- **Resultado Esperado:** retorno `"Outros"` sem `NullReferenceException`.
- **Criterios de Sucesso:** retorno `"Outros"` sem `NullReferenceException`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Entities - Orcamento

**Arquivo:** `tests/TransparenciaPE.UnitTests/Entities/OrcamentoTests.cs`

#### 1. Orcamento armazena campos orcamentarios

- **ID do Caso de Teste:** CT-013
- **Nome:** Orcamento armazena campos orcamentarios
- **Objetivo:** Validar o cenario: Orcamento armazena campos orcamentarios.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. `Ano=2026`, `DotacaoInicial=450000`, `DotacaoAtualizada=500000`.
- **Passos para Execucao:**
  1. instanciar `Orcamento` com os valores.
- **Resultado Esperado:** propriedades retornam exatamente os valores informados.
- **Criterios de Sucesso:** propriedades retornam exatamente os valores informados.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Orcamento armazena identificador do orgao

- **ID do Caso de Teste:** CT-014
- **Nome:** Orcamento armazena identificador do orgao
- **Objetivo:** Validar o cenario: Orcamento armazena identificador do orgao.
- **Pre-condicoes:** `Guid` gerado para representar o orgao de governo.
- **Entradas:**
  1. `OrgaoGovernoId=<Guid>`.
- **Passos para Execucao:**
  1. instanciar `Orcamento` atribuindo o identificador.
- **Resultado Esperado:** `OrgaoGovernoId` permanece igual ao `Guid` atribuido.
- **Criterios de Sucesso:** `OrgaoGovernoId` permanece igual ao `Guid` atribuido.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Orcamento gera identificador proprio

- **ID do Caso de Teste:** CT-015
- **Nome:** Orcamento gera identificador proprio
- **Objetivo:** Validar o cenario: Orcamento gera identificador proprio.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. nova instancia sem propriedades informadas.
- **Passos para Execucao:**
  1. criar `new Orcamento()`.
- **Resultado Esperado:** `Id` nao e `Guid.Empty`.
- **Criterios de Sucesso:** `Id` nao e `Guid.Empty`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Entities - OrgaoGoverno

**Arquivo:** `tests/TransparenciaPE.UnitTests/Entities/OrgaoGovernoTests.cs`

#### 1. OrgaoGoverno gera IDs unicos

- **ID do Caso de Teste:** CT-016
- **Nome:** OrgaoGoverno gera IDs unicos
- **Objetivo:** Validar o cenario: OrgaoGoverno gera IDs unicos.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. duas instancias independentes de `OrgaoGoverno`.
- **Passos para Execucao:**
  1. criar as duas instancias e comparar seus IDs.
- **Resultado Esperado:** ambos os IDs sao nao vazios e diferentes entre si.
- **Criterios de Sucesso:** ambos os IDs sao nao vazios e diferentes entre si.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. OrgaoGoverno inicializa colecoes de navegacao

- **ID do Caso de Teste:** CT-017
- **Nome:** OrgaoGoverno inicializa colecoes de navegacao
- **Objetivo:** Validar o cenario: OrgaoGoverno inicializa colecoes de navegacao.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. nova instancia vazia.
- **Passos para Execucao:**
  1. acessar `Empenhos` e `Contratos`.
- **Resultado Esperado:** colecoes sao nao nulas e vazias.
- **Criterios de Sucesso:** colecoes sao nao nulas e vazias.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. OrgaoGoverno define CreatedAt na criacao

- **ID do Caso de Teste:** CT-018
- **Nome:** OrgaoGoverno define CreatedAt na criacao
- **Objetivo:** Validar o cenario: OrgaoGoverno define CreatedAt na criacao.
- **Pre-condicoes:** janela de tempo registrada antes e depois da instanciacao.
- **Entradas:**
  1. nova instancia vazia.
- **Passos para Execucao:**
  1. criar o orgao e validar `CreatedAt` dentro da janela.
- **Resultado Esperado:** `CreatedAt` e recente e `UpdatedAt` inicia nulo.
- **Criterios de Sucesso:** `CreatedAt` e recente e `UpdatedAt` inicia nulo.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. OrgaoGoverno armazena dados de identificacao

- **ID do Caso de Teste:** CT-019
- **Nome:** OrgaoGoverno armazena dados de identificacao
- **Objetivo:** Validar o cenario: OrgaoGoverno armazena dados de identificacao.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. codigo, nome, sigla e tipo.
- **Passos para Execucao:**
  1. criar a entidade com os valores de identificacao.
- **Resultado Esperado:** todas as propriedades retornam os valores atribuidos.
- **Criterios de Sucesso:** todas as propriedades retornam os valores atribuidos.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. OrgaoGoverno armazena dados de capacidade

- **ID do Caso de Teste:** CT-020
- **Nome:** OrgaoGoverno armazena dados de capacidade
- **Objetivo:** Validar o cenario: OrgaoGoverno armazena dados de capacidade.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. `TotalServidores=1500` e `OrcamentoAtual=5000000`.
- **Passos para Execucao:**
  1. criar a entidade com esses valores.
- **Resultado Esperado:** campos numericos sao preservados sem perda.
- **Criterios de Sucesso:** campos numericos sao preservados sem perda.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Entities - Receita

**Arquivo:** `tests/TransparenciaPE.UnitTests/Entities/ReceitaTests.cs`

#### 1. Receita armazena campos contabeis

- **ID do Caso de Teste:** CT-021
- **Nome:** Receita armazena campos contabeis
- **Objetivo:** Validar o cenario: Receita armazena campos contabeis.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. valor, mes, ano e origem.
- **Passos para Execucao:**
  1. instanciar `Receita` com os dados contabeis.
- **Resultado Esperado:** propriedades retornam exatamente os valores atribuidos.
- **Criterios de Sucesso:** propriedades retornam exatamente os valores atribuidos.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Receita armazena identificador do orgao

- **ID do Caso de Teste:** CT-022
- **Nome:** Receita armazena identificador do orgao
- **Objetivo:** Validar o cenario: Receita armazena identificador do orgao.
- **Pre-condicoes:** `Guid` valido para orgao.
- **Entradas:**
  1. `OrgaoGovernoId=<Guid>`.
- **Passos para Execucao:**
  1. atribuir o identificador na instanciacao.
- **Resultado Esperado:** `OrgaoGovernoId` e igual ao valor recebido.
- **Criterios de Sucesso:** `OrgaoGovernoId` e igual ao valor recebido.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Receita gera identificador proprio

- **ID do Caso de Teste:** CT-023
- **Nome:** Receita gera identificador proprio
- **Objetivo:** Validar o cenario: Receita gera identificador proprio.
- **Pre-condicoes:** Nenhuma.
- **Entradas:**
  1. nova instancia vazia.
- **Passos para Execucao:**
  1. criar `new Receita()`.
- **Resultado Esperado:** `Id` nao e vazio.
- **Criterios de Sucesso:** `Id` nao e vazio.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Services - DashboardService

**Arquivo:** `tests/TransparenciaPE.UnitTests/Services/DashboardServiceTests.cs`

#### 1. Resumo retorna KPIs quando ha dados

- **ID do Caso de Teste:** CT-024
- **Nome:** Resumo retorna KPIs quando ha dados
- **Objetivo:** Validar o cenario: Resumo retorna KPIs quando ha dados.
- **Pre-condicoes:** `IDashboardQueryService` mockado com totais de empenhado, liquidado, pago, empenhos e contratos.
- **Entradas:**
  1. `TotalEmpenhado=1000000`, `TotalPago=600000`.
- **Passos para Execucao:**
  1. chamar `_sut.GetResumoAsync()` e inspecionar o DTO.
- **Resultado Esperado:** totais sao mapeados e `PercentualExecutado=60`.
- **Criterios de Sucesso:** totais sao mapeados e `PercentualExecutado=60`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Resumo evita divisao por zero

- **ID do Caso de Teste:** CT-025
- **Nome:** Resumo evita divisao por zero
- **Objetivo:** Validar o cenario: Resumo evita divisao por zero.
- **Pre-condicoes:** query retorna `TotalEmpenhado=0`.
- **Entradas:**
  1. totais zerados.
- **Passos para Execucao:**
  1. chamar `_sut.GetResumoAsync()`.
- **Resultado Esperado:** `PercentualExecutado` retorna `0`.
- **Criterios de Sucesso:** `PercentualExecutado` retorna `0`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Resumo repassa filtro de ano

- **ID do Caso de Teste:** CT-026
- **Nome:** Resumo repassa filtro de ano
- **Objetivo:** Validar o cenario: Resumo repassa filtro de ano.
- **Pre-condicoes:** mock configurado para receber `2025`.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `_sut.GetResumoAsync(2025)`.
- **Resultado Esperado:** `GetResumoAsync(2025)` do query service e chamado uma vez.
- **Criterios de Sucesso:** `GetResumoAsync(2025)` do query service e chamado uma vez.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Comparativo retorna ano solicitado

- **ID do Caso de Teste:** CT-027
- **Nome:** Comparativo retorna ano solicitado
- **Objetivo:** Validar o cenario: Comparativo retorna ano solicitado.
- **Pre-condicoes:** query retorna lista vazia.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `_sut.GetComparativoOrgaosAsync(2025)`.
- **Resultado Esperado:** DTO retorna `Ano=2025`.
- **Criterios de Sucesso:** DTO retorna `Ano=2025`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Comparativo retorna lista de orgaos

- **ID do Caso de Teste:** CT-028
- **Nome:** Comparativo retorna lista de orgaos
- **Objetivo:** Validar o cenario: Comparativo retorna lista de orgaos.
- **Pre-condicoes:** query retorna dois orgaos com valores de execucao.
- **Entradas:**
  1. orgaos `001` e `002`.
- **Passos para Execucao:**
  1. chamar o comparativo e contar os itens.
- **Resultado Esperado:** `Orgaos` possui dois itens mapeados.
- **Criterios de Sucesso:** `Orgaos` possui dois itens mapeados.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Drill-down retorna codigo do orgao

- **ID do Caso de Teste:** CT-029
- **Nome:** Drill-down retorna codigo do orgao
- **Objetivo:** Validar o cenario: Drill-down retorna codigo do orgao.
- **Pre-condicoes:** query retorna lista vazia.
- **Entradas:**
  1. `codigoOrgao="001"`.
- **Passos para Execucao:**
  1. chamar `_sut.GetDrillDownAsync("001")`.
- **Resultado Esperado:** DTO retorna `CodigoOrgao="001"`.
- **Criterios de Sucesso:** DTO retorna `CodigoOrgao="001"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. Drill-down retorna itens hierarquicos

- **ID do Caso de Teste:** CT-030
- **Nome:** Drill-down retorna itens hierarquicos
- **Objetivo:** Validar o cenario: Drill-down retorna itens hierarquicos.
- **Pre-condicoes:** query retorna um item de classificacao MCASP.
- **Entradas:**
  1. classificacao `3.3.90.30`, descricao e total.
- **Passos para Execucao:**
  1. chamar o drill-down e verificar a colecao.
- **Resultado Esperado:** `Itens` contem exatamente um item.
- **Criterios de Sucesso:** `Itens` contem exatamente um item.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Services - PesquisaService

**Arquivo:** `tests/TransparenciaPE.UnitTests/Services/PesquisaServiceTests.cs`

#### 1. Pesquisa global usa fluxo de CNPJ

- **ID do Caso de Teste:** CT-031
- **Nome:** Pesquisa global usa fluxo de CNPJ
- **Objetivo:** Validar o cenario: Pesquisa global usa fluxo de CNPJ.
- **Pre-condicoes:** repositorio de contratos mockado para CNPJ sanitizado.
- **Entradas:**
  1. CNPJ com e sem mascara.
- **Passos para Execucao:**
  1. chamar `PesquisaGlobalAsync(termo)`.
- **Resultado Esperado:** busca por CNPJ e executada uma vez e busca por fornecedor nao e executada.
- **Criterios de Sucesso:** busca por CNPJ e executada uma vez e busca por fornecedor nao e executada.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Pesquisa global usa fluxo textual

- **ID do Caso de Teste:** CT-032
- **Nome:** Pesquisa global usa fluxo textual
- **Objetivo:** Validar o cenario: Pesquisa global usa fluxo textual.
- **Pre-condicoes:** repositorio de contratos mockado para fornecedor.
- **Entradas:**
  1. `termo="Empresa ABC"`.
- **Passos para Execucao:**
  1. executar pesquisa global com termo textual.
- **Resultado Esperado:** `SearchByFornecedorAsync("Empresa ABC")` e chamado uma vez e termo buscado e preservado.
- **Criterios de Sucesso:** `SearchByFornecedorAsync("Empresa ABC")` e chamado uma vez e termo buscado e preservado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Pesquisa global rejeita termo vazio

- **ID do Caso de Teste:** CT-033
- **Nome:** Pesquisa global rejeita termo vazio
- **Objetivo:** Validar o cenario: Pesquisa global rejeita termo vazio.
- **Pre-condicoes:** Nenhuma chamada aos repositorios deve ser necessaria.
- **Entradas:**
  1. `""`, `null` e `"   "`.
- **Passos para Execucao:**
  1. chamar `PesquisaGlobalAsync` com cada valor invalido.
- **Resultado Esperado:** `ArgumentException` e lancada.
- **Criterios de Sucesso:** `ArgumentException` e lancada.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Exportacao CSV retorna dados de empenho

- **ID do Caso de Teste:** CT-034
- **Nome:** Exportacao CSV retorna dados de empenho
- **Objetivo:** Validar o cenario: Exportacao CSV retorna dados de empenho.
- **Pre-condicoes:** repositorio de empenhos retorna um empenho com orgao e credor.
- **Entradas:**
  1. `NumeroEmpenho="EMP-001"` e `Credor="Empresa A"`.
- **Passos para Execucao:**
  1. chamar `ExportarCsvAsync()` e decodificar bytes UTF-8.
- **Resultado Esperado:** CSV contem numero do empenho e fornecedor.
- **Criterios de Sucesso:** CSV contem numero do empenho e fornecedor.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Exportacao CSV retorna apenas cabecalho sem dados

- **ID do Caso de Teste:** CT-035
- **Nome:** Exportacao CSV retorna apenas cabecalho sem dados
- **Objetivo:** Validar o cenario: Exportacao CSV retorna apenas cabecalho sem dados.
- **Pre-condicoes:** repositorio retorna colecao vazia.
- **Entradas:**
  1. nenhum empenho.
- **Passos para Execucao:**
  1. chamar `ExportarCsvAsync()` e separar linhas.
- **Resultado Esperado:** CSV contem somente a linha de cabecalho.
- **Criterios de Sucesso:** CSV contem somente a linha de cabecalho.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Exportacao CSV aplica filtro de ano

- **ID do Caso de Teste:** CT-036
- **Nome:** Exportacao CSV aplica filtro de ano
- **Objetivo:** Validar o cenario: Exportacao CSV aplica filtro de ano.
- **Pre-condicoes:** callback captura a expressao enviada ao repositorio.
- **Entradas:**
  1. filtro `ano=2025`.
- **Passos para Execucao:**
  1. compilar o predicado capturado e testar empenhos de 2025 e 2024.
- **Resultado Esperado:** predicado aceita 2025 e rejeita 2024.
- **Criterios de Sucesso:** predicado aceita 2025 e rejeita 2024.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Services - DataSyncService

**Arquivo:** `tests/TransparenciaPE.UnitTests/Services/DataSyncServiceTests.cs`

#### 1. Sincronizacao insere empenho novo

- **ID do Caso de Teste:** CT-037
- **Nome:** Sincronizacao insere empenho novo
- **Objetivo:** Validar o cenario: Sincronizacao insere empenho novo.
- **Pre-condicoes:** existe orgao base e API externa retorna empenho inexistente.
- **Entradas:**
  1. `NumeroEmpenho="EMP-001"`, ano 2025 e CNPJ com mascara.
- **Passos para Execucao:**
  1. chamar `SyncEmpenhosAsync(2025)`.
- **Resultado Esperado:** empenho e adicionado com CNPJ sanitizado, count retorna `1` e commit e chamado.
- **Criterios de Sucesso:** empenho e adicionado com CNPJ sanitizado, count retorna `1` e commit e chamado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Sincronizacao atualiza empenho existente

- **ID do Caso de Teste:** CT-038
- **Nome:** Sincronizacao atualiza empenho existente
- **Objetivo:** Validar o cenario: Sincronizacao atualiza empenho existente.
- **Pre-condicoes:** repositorio retorna empenho ja existente.
- **Entradas:**
  1. valor antigo `30000` e valor externo `50000`.
- **Passos para Execucao:**
  1. executar sincronizacao do ano.
- **Resultado Esperado:** valor do empenho existente e atualizado e nenhum novo empenho e inserido.
- **Criterios de Sucesso:** valor do empenho existente e atualizado e nenhum novo empenho e inserido.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Sincronizacao sanitiza CNPJ ao adicionar empenho

- **ID do Caso de Teste:** CT-039
- **Nome:** Sincronizacao sanitiza CNPJ ao adicionar empenho
- **Objetivo:** Validar o cenario: Sincronizacao sanitiza CNPJ ao adicionar empenho.
- **Pre-condicoes:** API retorna empenho novo com CNPJ pontuado.
- **Entradas:**
  1. `"11.222.333/0001-81"`.
- **Passos para Execucao:**
  1. sincronizar empenhos.
- **Resultado Esperado:** `AddAsync` recebe entidade com `CnpjCredor="11222333000181"`.
- **Criterios de Sucesso:** `AddAsync` recebe entidade com `CnpjCredor="11222333000181"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. SyncAll retorna timestamp de sincronizacao

- **ID do Caso de Teste:** CT-040
- **Nome:** SyncAll retorna timestamp de sincronizacao
- **Objetivo:** Validar o cenario: SyncAll retorna timestamp de sincronizacao.
- **Pre-condicoes:** APIs retornam listas vazias.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `SyncAllAsync(2025)`.
- **Resultado Esperado:** `SyncedAt` e preenchido com data/hora menor ou igual ao momento atual.
- **Criterios de Sucesso:** `SyncedAt` e preenchido com data/hora menor ou igual ao momento atual.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Sincronizacao de empenhos para quando nao ha orgaos

- **ID do Caso de Teste:** CT-041
- **Nome:** Sincronizacao de empenhos para quando nao ha orgaos
- **Objetivo:** Validar o cenario: Sincronizacao de empenhos para quando nao ha orgaos.
- **Pre-condicoes:** repositorio de orgaos retorna lista vazia.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `SyncEmpenhosAsync`.
- **Resultado Esperado:** retorna `0`, nao consulta API externa e nao realiza commit.
- **Criterios de Sucesso:** retorna `0`, nao consulta API externa e nao realiza commit.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Sincronizacao retorna zero quando API nao traz empenhos

- **ID do Caso de Teste:** CT-042
- **Nome:** Sincronizacao retorna zero quando API nao traz empenhos
- **Objetivo:** Validar o cenario: Sincronizacao retorna zero quando API nao traz empenhos.
- **Pre-condicoes:** existe orgao base, mas API retorna lista vazia.
- **Entradas:**
  1. orgao `001`.
- **Passos para Execucao:**
  1. chamar sincronizacao.
- **Resultado Esperado:** retorna `0`, nao adiciona empenhos e realiza commit da operacao.
- **Criterios de Sucesso:** retorna `0`, nao adiciona empenhos e realiza commit da operacao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. Sincronizacao insere contrato novo

- **ID do Caso de Teste:** CT-043
- **Nome:** Sincronizacao insere contrato novo
- **Objetivo:** Validar o cenario: Sincronizacao insere contrato novo.
- **Pre-condicoes:** API externa retorna contrato inexistente.
- **Entradas:**
  1. contrato `CT-2025-001` com CNPJ pontuado.
- **Passos para Execucao:**
  1. chamar `SyncContratosAsync(2025)`.
- **Resultado Esperado:** contrato e adicionado com CNPJ sanitizado, count `1` e commit chamado.
- **Criterios de Sucesso:** contrato e adicionado com CNPJ sanitizado, count `1` e commit chamado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 8. SyncAll agrega contadores processados

- **ID do Caso de Teste:** CT-044
- **Nome:** SyncAll agrega contadores processados
- **Objetivo:** Validar o cenario: SyncAll agrega contadores processados.
- **Pre-condicoes:** API retorna um empenho novo e um contrato novo.
- **Entradas:**
  1. `EMP-X` e `CT-X`.
- **Passos para Execucao:**
  1. executar `SyncAllAsync(2025)`.
- **Resultado Esperado:** resultado retorna `EmpenhosProcessados=1` e `ContratosProcessados=1`.
- **Criterios de Sucesso:** resultado retorna `EmpenhosProcessados=1` e `ContratosProcessados=1`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Controllers - DashboardController

**Arquivo:** `tests/TransparenciaPE.UnitTests/Controllers/DashboardControllerTests.cs`

#### 1. GetResumo retorna Ok

- **ID do Caso de Teste:** CT-045
- **Nome:** GetResumo retorna Ok
- **Objetivo:** Validar o cenario: GetResumo retorna Ok.
- **Pre-condicoes:** `IDashboardService` retorna `DashboardResumoDto`.
- **Entradas:**
  1. chamada sem ano.
- **Passos para Execucao:**
  1. executar `_sut.GetResumo(null)`.
- **Resultado Esperado:** resultado HTTP e `OkObjectResult`.
- **Criterios de Sucesso:** resultado HTTP e `OkObjectResult`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. GetResumo retorna DTO esperado

- **ID do Caso de Teste:** CT-046
- **Nome:** GetResumo retorna DTO esperado
- **Objetivo:** Validar o cenario: GetResumo retorna DTO esperado.
- **Pre-condicoes:** servico retorna resumo vazio valido.
- **Entradas:**
  1. chamada sem ano.
- **Passos para Execucao:**
  1. executar controller e inspecionar `Value`.
- **Resultado Esperado:** corpo e do tipo `DashboardResumoDto`.
- **Criterios de Sucesso:** corpo e do tipo `DashboardResumoDto`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. GetResumo propaga total empenhado

- **ID do Caso de Teste:** CT-047
- **Nome:** GetResumo propaga total empenhado
- **Objetivo:** Validar o cenario: GetResumo propaga total empenhado.
- **Pre-condicoes:** servico retorna `TotalEmpenhado=1000000`.
- **Entradas:**
  1. chamada sem ano.
- **Passos para Execucao:**
  1. executar controller e converter corpo para DTO.
- **Resultado Esperado:** DTO retornado contem `TotalEmpenhado=1000000`.
- **Criterios de Sucesso:** DTO retornado contem `TotalEmpenhado=1000000`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. GetResumo chama servico com ano nulo

- **ID do Caso de Teste:** CT-048
- **Nome:** GetResumo chama servico com ano nulo
- **Objetivo:** Validar o cenario: GetResumo chama servico com ano nulo.
- **Pre-condicoes:** mock configurado para `GetResumoAsync(null)`.
- **Entradas:**
  1. `ano=null`.
- **Passos para Execucao:**
  1. executar endpoint do controller.
- **Resultado Esperado:** servico e chamado uma vez com `null`.
- **Criterios de Sucesso:** servico e chamado uma vez com `null`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. GetResumo chama servico com ano informado

- **ID do Caso de Teste:** CT-049
- **Nome:** GetResumo chama servico com ano informado
- **Objetivo:** Validar o cenario: GetResumo chama servico com ano informado.
- **Pre-condicoes:** mock configurado para `2025`.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. executar endpoint do controller.
- **Resultado Esperado:** servico e chamado uma vez com `2025`.
- **Criterios de Sucesso:** servico e chamado uma vez com `2025`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. GetComparativo retorna Ok

- **ID do Caso de Teste:** CT-050
- **Nome:** GetComparativo retorna Ok
- **Objetivo:** Validar o cenario: GetComparativo retorna Ok.
- **Pre-condicoes:** servico retorna `ComparativoOrgaosDto`.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `_sut.GetComparativo(2025)`.
- **Resultado Esperado:** resultado HTTP e `OkObjectResult`.
- **Criterios de Sucesso:** resultado HTTP e `OkObjectResult`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. GetComparativo retorna DTO esperado

- **ID do Caso de Teste:** CT-051
- **Nome:** GetComparativo retorna DTO esperado
- **Objetivo:** Validar o cenario: GetComparativo retorna DTO esperado.
- **Pre-condicoes:** servico retorna comparativo valido.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. executar controller e avaliar corpo.
- **Resultado Esperado:** corpo e `ComparativoOrgaosDto`.
- **Criterios de Sucesso:** corpo e `ComparativoOrgaosDto`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 8. GetComparativo propaga ano

- **ID do Caso de Teste:** CT-052
- **Nome:** GetComparativo propaga ano
- **Objetivo:** Validar o cenario: GetComparativo propaga ano.
- **Pre-condicoes:** servico retorna DTO com `Ano=2025`.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. executar controller.
- **Resultado Esperado:** corpo retorna `Ano=2025`.
- **Criterios de Sucesso:** corpo retorna `Ano=2025`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 9. GetEvolucao retorna Ok

- **ID do Caso de Teste:** CT-053
- **Nome:** GetEvolucao retorna Ok
- **Objetivo:** Validar o cenario: GetEvolucao retorna Ok.
- **Pre-condicoes:** servico retorna `DrillDownDto`.
- **Entradas:**
  1. `codigoOrgao="001"`.
- **Passos para Execucao:**
  1. chamar `_sut.GetEvolucao("001", null)`.
- **Resultado Esperado:** resultado HTTP e `OkObjectResult`.
- **Criterios de Sucesso:** resultado HTTP e `OkObjectResult`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 10. GetEvolucao retorna DTO de drill-down

- **ID do Caso de Teste:** CT-054
- **Nome:** GetEvolucao retorna DTO de drill-down
- **Objetivo:** Validar o cenario: GetEvolucao retorna DTO de drill-down.
- **Pre-condicoes:** servico retorna drill-down valido.
- **Entradas:**
  1. `codigoOrgao="001"`.
- **Passos para Execucao:**
  1. executar controller e inspecionar corpo.
- **Resultado Esperado:** corpo e `DrillDownDto`.
- **Criterios de Sucesso:** corpo e `DrillDownDto`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 11. GetEvolucao propaga codigo do orgao

- **ID do Caso de Teste:** CT-055
- **Nome:** GetEvolucao propaga codigo do orgao
- **Objetivo:** Validar o cenario: GetEvolucao propaga codigo do orgao.
- **Pre-condicoes:** servico retorna DTO com `CodigoOrgao="001"`.
- **Entradas:**
  1. `codigoOrgao="001"`.
- **Passos para Execucao:**
  1. executar controller.
- **Resultado Esperado:** corpo preserva `CodigoOrgao="001"`.
- **Criterios de Sucesso:** corpo preserva `CodigoOrgao="001"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 12. GetEvolucao chama servico com ano nulo

- **ID do Caso de Teste:** CT-056
- **Nome:** GetEvolucao chama servico com ano nulo
- **Objetivo:** Validar o cenario: GetEvolucao chama servico com ano nulo.
- **Pre-condicoes:** mock configurado com codigo `001` e ano nulo.
- **Entradas:**
  1. `codigoOrgao="001"`, `ano=null`.
- **Passos para Execucao:**
  1. chamar controller.
- **Resultado Esperado:** servico recebe exatamente esses parametros.
- **Criterios de Sucesso:** servico recebe exatamente esses parametros.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 13. GetEvolucao chama servico com parametros informados

- **ID do Caso de Teste:** CT-057
- **Nome:** GetEvolucao chama servico com parametros informados
- **Objetivo:** Validar o cenario: GetEvolucao chama servico com parametros informados.
- **Pre-condicoes:** mock configurado com codigo `002` e ano `2025`.
- **Entradas:**
  1. `codigoOrgao="002"`, `ano=2025`.
- **Passos para Execucao:**
  1. chamar controller.
- **Resultado Esperado:** servico e chamado uma vez com codigo e ano informados.
- **Criterios de Sucesso:** servico e chamado uma vez com codigo e ano informados.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Controllers - PesquisaController

**Arquivo:** `tests/TransparenciaPE.UnitTests/Controllers/PesquisaControllerTests.cs`

#### 1. PesquisaGlobal retorna Ok

- **ID do Caso de Teste:** CT-058
- **Nome:** PesquisaGlobal retorna Ok
- **Objetivo:** Validar o cenario: PesquisaGlobal retorna Ok.
- **Pre-condicoes:** servico retorna `PesquisaResultDto`.
- **Entradas:**
  1. termo `"Empresa A"`.
- **Passos para Execucao:**
  1. chamar `_sut.PesquisaGlobal("Empresa A")`.
- **Resultado Esperado:** resultado HTTP e `OkObjectResult`.
- **Criterios de Sucesso:** resultado HTTP e `OkObjectResult`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. PesquisaGlobal retorna DTO esperado

- **ID do Caso de Teste:** CT-059
- **Nome:** PesquisaGlobal retorna DTO esperado
- **Objetivo:** Validar o cenario: PesquisaGlobal retorna DTO esperado.
- **Pre-condicoes:** servico retorna resultado valido.
- **Entradas:**
  1. termo `"Empresa A"`.
- **Passos para Execucao:**
  1. executar controller e inspecionar corpo.
- **Resultado Esperado:** corpo e `PesquisaResultDto`.
- **Criterios de Sucesso:** corpo e `PesquisaResultDto`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. PesquisaGlobal propaga termo buscado

- **ID do Caso de Teste:** CT-060
- **Nome:** PesquisaGlobal propaga termo buscado
- **Objetivo:** Validar o cenario: PesquisaGlobal propaga termo buscado.
- **Pre-condicoes:** servico retorna `TermoBuscado="Empresa A"`.
- **Entradas:**
  1. termo `"Empresa A"`.
- **Passos para Execucao:**
  1. executar controller.
- **Resultado Esperado:** corpo preserva o termo buscado.
- **Criterios de Sucesso:** corpo preserva o termo buscado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. PesquisaGlobal retorna status 400 para termo vazio

- **ID do Caso de Teste:** CT-061
- **Nome:** PesquisaGlobal retorna status 400 para termo vazio
- **Objetivo:** Validar o cenario: PesquisaGlobal retorna status 400 para termo vazio.
- **Pre-condicoes:** servico lanca `ArgumentException` para termo vazio.
- **Entradas:**
  1. `termo=""`.
- **Passos para Execucao:**
  1. chamar controller com termo vazio.
- **Resultado Esperado:** `BadRequestObjectResult.StatusCode=400`.
- **Criterios de Sucesso:** `BadRequestObjectResult.StatusCode=400`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. PesquisaGlobal retorna BadRequestObjectResult

- **ID do Caso de Teste:** CT-062
- **Nome:** PesquisaGlobal retorna BadRequestObjectResult
- **Objetivo:** Validar o cenario: PesquisaGlobal retorna BadRequestObjectResult.
- **Pre-condicoes:** servico lanca `ArgumentException`.
- **Entradas:**
  1. `termo=""`.
- **Passos para Execucao:**
  1. chamar controller.
- **Resultado Esperado:** `result.Result` e `BadRequestObjectResult`.
- **Criterios de Sucesso:** `result.Result` e `BadRequestObjectResult`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. ExportarCsv retorna arquivo

- **ID do Caso de Teste:** CT-063
- **Nome:** ExportarCsv retorna arquivo
- **Objetivo:** Validar o cenario: ExportarCsv retorna arquivo.
- **Pre-condicoes:** servico retorna bytes CSV.
- **Entradas:**
  1. chamada sem termo e sem ano.
- **Passos para Execucao:**
  1. chamar `_sut.ExportarCsv(null, null)`.
- **Resultado Esperado:** retorno e `FileContentResult`.
- **Criterios de Sucesso:** retorno e `FileContentResult`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. ExportarCsv define content-type CSV

- **ID do Caso de Teste:** CT-064
- **Nome:** ExportarCsv define content-type CSV
- **Objetivo:** Validar o cenario: ExportarCsv define content-type CSV.
- **Pre-condicoes:** servico retorna bytes CSV.
- **Entradas:**
  1. CSV simples em UTF-8.
- **Passos para Execucao:**
  1. chamar controller e ler `ContentType`.
- **Resultado Esperado:** `ContentType="text/csv"`.
- **Criterios de Sucesso:** `ContentType="text/csv"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 8. ExportarCsv define nome de arquivo

- **ID do Caso de Teste:** CT-065
- **Nome:** ExportarCsv define nome de arquivo
- **Objetivo:** Validar o cenario: ExportarCsv define nome de arquivo.
- **Pre-condicoes:** servico retorna bytes CSV.
- **Entradas:**
  1. chamada sem filtros.
- **Passos para Execucao:**
  1. chamar controller e ler `FileDownloadName`.
- **Resultado Esperado:** nome de download contem `.csv`.
- **Criterios de Sucesso:** nome de download contem `.csv`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Infrastructure - AppDbContext

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/AppDbContextTests.cs`

#### 1. Modelo aplica configuracao de Empenho

- **ID do Caso de Teste:** CT-066
- **Nome:** Modelo aplica configuracao de Empenho
- **Objetivo:** Validar o cenario: Modelo aplica configuracao de Empenho.
- **Pre-condicoes:** contexto em memoria criado.
- **Entradas:**
  1. metadados EF da entidade `Empenho`.
- **Passos para Execucao:**
  1. consultar tabela, tamanhos de campos e tipo decimal.
- **Resultado Esperado:** tabela `empenhos`, limites esperados e `numeric(18,2)`.
- **Criterios de Sucesso:** tabela `empenhos`, limites esperados e `numeric(18,2)`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Modelo aplica configuracao de Contrato

- **ID do Caso de Teste:** CT-067
- **Nome:** Modelo aplica configuracao de Contrato
- **Objetivo:** Validar o cenario: Modelo aplica configuracao de Contrato.
- **Pre-condicoes:** contexto em memoria criado.
- **Entradas:**
  1. metadados EF da entidade `Contrato`.
- **Passos para Execucao:**
  1. verificar tabela e limites de propriedades.
- **Resultado Esperado:** tabela `contratos`, numero com tamanho 50 e objeto com tamanho 1000.
- **Criterios de Sucesso:** tabela `contratos`, numero com tamanho 50 e objeto com tamanho 1000.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Modelo aplica configuracao de OrgaoGoverno

- **ID do Caso de Teste:** CT-068
- **Nome:** Modelo aplica configuracao de OrgaoGoverno
- **Objetivo:** Validar o cenario: Modelo aplica configuracao de OrgaoGoverno.
- **Pre-condicoes:** contexto em memoria criado.
- **Entradas:**
  1. metadados da entidade `OrgaoGoverno`.
- **Passos para Execucao:**
  1. verificar nome da tabela e limites.
- **Resultado Esperado:** tabela `orgaos_governo`, codigo ate 20 e nome ate 200.
- **Criterios de Sucesso:** tabela `orgaos_governo`, codigo ate 20 e nome ate 200.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Modelo aplica configuracao de Liquidacao

- **ID do Caso de Teste:** CT-069
- **Nome:** Modelo aplica configuracao de Liquidacao
- **Objetivo:** Validar o cenario: Modelo aplica configuracao de Liquidacao.
- **Pre-condicoes:** contexto em memoria criado.
- **Entradas:**
  1. metadados de `Liquidacao`.
- **Passos para Execucao:**
  1. verificar tabela e tamanho do numero.
- **Resultado Esperado:** tabela `liquidacoes` e numero com limite 50.
- **Criterios de Sucesso:** tabela `liquidacoes` e numero com limite 50.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Modelo aplica configuracao de Pagamento

- **ID do Caso de Teste:** CT-070
- **Nome:** Modelo aplica configuracao de Pagamento
- **Objetivo:** Validar o cenario: Modelo aplica configuracao de Pagamento.
- **Pre-condicoes:** contexto em memoria criado.
- **Entradas:**
  1. metadados de `Pagamento`.
- **Passos para Execucao:**
  1. verificar tabela e tamanho do numero.
- **Resultado Esperado:** tabela `pagamentos` e numero com limite 50.
- **Criterios de Sucesso:** tabela `pagamentos` e numero com limite 50.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. SaveChanges define CreatedAt ao adicionar

- **ID do Caso de Teste:** CT-071
- **Nome:** SaveChanges define CreatedAt ao adicionar
- **Objetivo:** Validar o cenario: SaveChanges define CreatedAt ao adicionar.
- **Pre-condicoes:** entidade criada com `CreatedAt` antigo.
- **Entradas:**
  1. orgao novo.
- **Passos para Execucao:**
  1. adicionar entidade e salvar.
- **Resultado Esperado:** `CreatedAt` e atualizado para valor recente e `UpdatedAt` permanece nulo.
- **Criterios de Sucesso:** `CreatedAt` e atualizado para valor recente e `UpdatedAt` permanece nulo.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. SaveChanges define UpdatedAt ao modificar

- **ID do Caso de Teste:** CT-072
- **Nome:** SaveChanges define UpdatedAt ao modificar
- **Objetivo:** Validar o cenario: SaveChanges define UpdatedAt ao modificar.
- **Pre-condicoes:** entidade ja persistida.
- **Entradas:**
  1. alteracao no nome do orgao.
- **Passos para Execucao:**
  1. atualizar entidade e salvar.
- **Resultado Esperado:** `UpdatedAt` e preenchido com data recente.
- **Criterios de Sucesso:** `UpdatedAt` e preenchido com data recente.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Infrastructure - FakePEDataClient

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/FakePEDataClientTests.cs`

#### 1. Fake retorna empenhos do ano solicitado

- **ID do Caso de Teste:** CT-073
- **Nome:** Fake retorna empenhos do ano solicitado
- **Objetivo:** Validar o cenario: Fake retorna empenhos do ano solicitado.
- **Pre-condicoes:** cliente fake instanciado.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `GetEmpenhosAsync(2025)`.
- **Resultado Esperado:** tres empenhos retornados e todos pertencem a 2025.
- **Criterios de Sucesso:** tres empenhos retornados e todos pertencem a 2025.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Fake filtra empenhos por orgao

- **ID do Caso de Teste:** CT-074
- **Nome:** Fake filtra empenhos por orgao
- **Objetivo:** Validar o cenario: Fake filtra empenhos por orgao.
- **Pre-condicoes:** cliente fake instanciado.
- **Entradas:**
  1. `ano=2025`, `codigoOrgao="SES"`.
- **Passos para Execucao:**
  1. chamar metodo de filtro por orgao.
- **Resultado Esperado:** retorna um item com `CodigoOrgao="SES"`.
- **Criterios de Sucesso:** retorna um item com `CodigoOrgao="SES"`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Fake retorna contratos do ano solicitado

- **ID do Caso de Teste:** CT-075
- **Nome:** Fake retorna contratos do ano solicitado
- **Objetivo:** Validar o cenario: Fake retorna contratos do ano solicitado.
- **Pre-condicoes:** cliente fake instanciado.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `GetContratosAsync`.
- **Resultado Esperado:** dois contratos com numero iniciado por `CT-2025`.
- **Criterios de Sucesso:** dois contratos com numero iniciado por `CT-2025`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Fake retorna receitas do ano solicitado

- **ID do Caso de Teste:** CT-076
- **Nome:** Fake retorna receitas do ano solicitado
- **Objetivo:** Validar o cenario: Fake retorna receitas do ano solicitado.
- **Pre-condicoes:** cliente fake instanciado.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `GetReceitasAsync`.
- **Resultado Esperado:** uma receita com ano 2025 e valor esperado.
- **Criterios de Sucesso:** uma receita com ano 2025 e valor esperado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Fake retorna orcamento do ano solicitado

- **ID do Caso de Teste:** CT-077
- **Nome:** Fake retorna orcamento do ano solicitado
- **Objetivo:** Validar o cenario: Fake retorna orcamento do ano solicitado.
- **Pre-condicoes:** cliente fake instanciado.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `GetOrcamentoAsync`.
- **Resultado Esperado:** um registro com dotacao atualizada esperada.
- **Criterios de Sucesso:** um registro com dotacao atualizada esperada.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Fake retorna total fixo de servidores

- **ID do Caso de Teste:** CT-078
- **Nome:** Fake retorna total fixo de servidores
- **Objetivo:** Validar o cenario: Fake retorna total fixo de servidores.
- **Pre-condicoes:** cliente fake instanciado.
- **Entradas:**
  1. `codigoOrgao="SEE"`.
- **Passos para Execucao:**
  1. chamar `GetTotalServidoresAsync`.
- **Resultado Esperado:** retorno `1200`.
- **Criterios de Sucesso:** retorno `1200`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Infrastructure - Repository

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/RepositoryTests.cs`

#### 1. Repositorio adiciona entidade

- **ID do Caso de Teste:** CT-079
- **Nome:** Repositorio adiciona entidade
- **Objetivo:** Validar o cenario: Repositorio adiciona entidade.
- **Pre-condicoes:** contexto em memoria vazio.
- **Entradas:**
  1. orgao `001`.
- **Passos para Execucao:**
  1. chamar `AddAsync` e `SaveChangesAsync`.
- **Resultado Esperado:** entidade passa a existir no `DbSet`.
- **Criterios de Sucesso:** entidade passa a existir no `DbSet`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Repositorio busca por ID

- **ID do Caso de Teste:** CT-080
- **Nome:** Repositorio busca por ID
- **Objetivo:** Validar o cenario: Repositorio busca por ID.
- **Pre-condicoes:** orgao previamente salvo.
- **Entradas:**
  1. ID do orgao salvo.
- **Passos para Execucao:**
  1. chamar `GetByIdAsync(id)`.
- **Resultado Esperado:** entidade retornada possui o mesmo ID.
- **Criterios de Sucesso:** entidade retornada possui o mesmo ID.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Repositorio lista entidades

- **ID do Caso de Teste:** CT-081
- **Nome:** Repositorio lista entidades
- **Objetivo:** Validar o cenario: Repositorio lista entidades.
- **Pre-condicoes:** orgao previamente salvo.
- **Entradas:**
  1. contexto com um orgao.
- **Passos para Execucao:**
  1. chamar `GetAllAsync()`.
- **Resultado Esperado:** lista contem o orgao salvo.
- **Criterios de Sucesso:** lista contem o orgao salvo.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Repositorio filtra por predicado

- **ID do Caso de Teste:** CT-082
- **Nome:** Repositorio filtra por predicado
- **Objetivo:** Validar o cenario: Repositorio filtra por predicado.
- **Pre-condicoes:** orgao salvo com codigo `001`.
- **Entradas:**
  1. predicado `o.Codigo == "001"`.
- **Passos para Execucao:**
  1. chamar `FindAsync`.
- **Resultado Esperado:** colecao contem somente o orgao esperado.
- **Criterios de Sucesso:** colecao contem somente o orgao esperado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Repositorio confirma existencia

- **ID do Caso de Teste:** CT-083
- **Nome:** Repositorio confirma existencia
- **Objetivo:** Validar o cenario: Repositorio confirma existencia.
- **Pre-condicoes:** orgao salvo com nome contendo `Educacao`.
- **Entradas:**
  1. predicado por nome.
- **Passos para Execucao:**
  1. chamar `ExistsAsync`.
- **Resultado Esperado:** retorno `true`.
- **Criterios de Sucesso:** retorno `true`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Repositorio atualiza entidade

- **ID do Caso de Teste:** CT-084
- **Nome:** Repositorio atualiza entidade
- **Objetivo:** Validar o cenario: Repositorio atualiza entidade.
- **Pre-condicoes:** orgao salvo.
- **Entradas:**
  1. novo nome `Secretaria Estadual de Educacao`.
- **Passos para Execucao:**
  1. alterar propriedade, chamar `Update` e salvar.
- **Resultado Esperado:** busca posterior retorna o nome atualizado.
- **Criterios de Sucesso:** busca posterior retorna o nome atualizado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. Repositorio remove entidade

- **ID do Caso de Teste:** CT-085
- **Nome:** Repositorio remove entidade
- **Objetivo:** Validar o cenario: Repositorio remove entidade.
- **Pre-condicoes:** orgao salvo.
- **Entradas:**
  1. ID do orgao.
- **Passos para Execucao:**
  1. chamar `Remove`, salvar e verificar existencia.
- **Resultado Esperado:** `ExistsAsync` retorna `false`.
- **Criterios de Sucesso:** `ExistsAsync` retorna `false`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 8. EmpenhoRepository busca empenho com orgao

- **ID do Caso de Teste:** CT-086
- **Nome:** EmpenhoRepository busca empenho com orgao
- **Objetivo:** Validar o cenario: EmpenhoRepository busca empenho com orgao.
- **Pre-condicoes:** empenho e orgao salvos no contexto.
- **Entradas:**
  1. numero `EMP-001`, ano `2025`.
- **Passos para Execucao:**
  1. chamar `GetByNumeroAsync`.
- **Resultado Esperado:** empenho retornado inclui dados do orgao.
- **Criterios de Sucesso:** empenho retornado inclui dados do orgao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 9. EmpenhoRepository filtra por ano

- **ID do Caso de Teste:** CT-087
- **Nome:** EmpenhoRepository filtra por ano
- **Objetivo:** Validar o cenario: EmpenhoRepository filtra por ano.
- **Pre-condicoes:** existem empenhos de 2025 e 2024.
- **Entradas:**
  1. `ano=2025`.
- **Passos para Execucao:**
  1. chamar `GetByAnoAsync(2025)`.
- **Resultado Esperado:** retorna apenas o empenho de 2025.
- **Criterios de Sucesso:** retorna apenas o empenho de 2025.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 10. EmpenhoRepository filtra por orgao

- **ID do Caso de Teste:** CT-088
- **Nome:** EmpenhoRepository filtra por orgao
- **Objetivo:** Validar o cenario: EmpenhoRepository filtra por orgao.
- **Pre-condicoes:** existem empenhos de dois orgaos.
- **Entradas:**
  1. ID do orgao de educacao.
- **Passos para Execucao:**
  1. chamar `GetByOrgaoAsync`.
- **Resultado Esperado:** todos os itens retornados pertencem ao orgao informado.
- **Criterios de Sucesso:** todos os itens retornados pertencem ao orgao informado.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 11. ContratoRepository busca contrato com orgao

- **ID do Caso de Teste:** CT-089
- **Nome:** ContratoRepository busca contrato com orgao
- **Objetivo:** Validar o cenario: ContratoRepository busca contrato com orgao.
- **Pre-condicoes:** contrato e orgao salvos.
- **Entradas:**
  1. numero `CT-001`.
- **Passos para Execucao:**
  1. chamar `GetByNumeroAsync`.
- **Resultado Esperado:** contrato retornado inclui fornecedor e orgao.
- **Criterios de Sucesso:** contrato retornado inclui fornecedor e orgao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 12. ContratoRepository busca por CNPJ

- **ID do Caso de Teste:** CT-090
- **Nome:** ContratoRepository busca por CNPJ
- **Objetivo:** Validar o cenario: ContratoRepository busca por CNPJ.
- **Pre-condicoes:** existem contratos com CNPJs diferentes.
- **Entradas:**
  1. CNPJ `"11222333000181"`.
- **Passos para Execucao:**
  1. chamar `SearchByCnpjAsync`.
- **Resultado Esperado:** retorna somente o contrato com CNPJ correspondente.
- **Criterios de Sucesso:** retorna somente o contrato com CNPJ correspondente.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Infrastructure - UnitOfWork

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/UnitOfWorkTests.cs`

#### 1. UnitOfWork expoe repositorios configurados

- **ID do Caso de Teste:** CT-091
- **Nome:** UnitOfWork expoe repositorios configurados
- **Objetivo:** Validar o cenario: UnitOfWork expoe repositorios configurados.
- **Pre-condicoes:** repositorios de empenhos e contratos instanciados.
- **Entradas:**
  1. contexto em memoria.
- **Passos para Execucao:**
  1. criar `UnitOfWork`.
- **Resultado Esperado:** propriedades `Empenhos` e `Contratos` referenciam os repositorios recebidos.
- **Criterios de Sucesso:** propriedades `Empenhos` e `Contratos` referenciam os repositorios recebidos.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. UnitOfWork persiste alteracoes pendentes

- **ID do Caso de Teste:** CT-092
- **Nome:** UnitOfWork persiste alteracoes pendentes
- **Objetivo:** Validar o cenario: UnitOfWork persiste alteracoes pendentes.
- **Pre-condicoes:** entidade adicionada ao contexto.
- **Entradas:**
  1. orgao novo.
- **Passos para Execucao:**
  1. chamar `CommitAsync`.
- **Resultado Esperado:** retorno indica uma alteracao persistida e entidade fica salva.
- **Criterios de Sucesso:** retorno indica uma alteracao persistida e entidade fica salva.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. UnitOfWork descarta contexto

- **ID do Caso de Teste:** CT-093
- **Nome:** UnitOfWork descarta contexto
- **Objetivo:** Validar o cenario: UnitOfWork descarta contexto.
- **Pre-condicoes:** `UnitOfWork` criado com contexto em memoria.
- **Entradas:**
  1. chamada direta a `Dispose`.
- **Passos para Execucao:**
  1. descartar e tentar acessar o contexto.
- **Resultado Esperado:** acesso posterior lanca `ObjectDisposedException`.
- **Criterios de Sucesso:** acesso posterior lanca `ObjectDisposedException`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de ExternalClients - TcePEDataClient

**Arquivo:** `tests/TransparenciaPE.UnitTests/ExternalClients/TcePEDataClientTests.cs`

#### 1. Cliente TCE desserializa receitas

- **ID do Caso de Teste:** CT-094
- **Nome:** Cliente TCE desserializa receitas
- **Objetivo:** Validar o cenario: Cliente TCE desserializa receitas.
- **Pre-condicoes:** `HttpMessageHandler` mockado retorna HTTP 200.
- **Entradas:**
  1. envelope JSON do TCE com uma receita.
- **Passos para Execucao:**
  1. chamar `GetReceitasAsync(2026)`.
- **Resultado Esperado:** lista contem uma receita com valor e origem esperados.
- **Criterios de Sucesso:** lista contem uma receita com valor e origem esperados.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Cliente TCE lanca excecao em erro de receitas

- **ID do Caso de Teste:** CT-095
- **Nome:** Cliente TCE lanca excecao em erro de receitas
- **Objetivo:** Validar o cenario: Cliente TCE lanca excecao em erro de receitas.
- **Pre-condicoes:** handler retorna HTTP 500.
- **Entradas:**
  1. corpo vazio/default.
- **Passos para Execucao:**
  1. chamar `GetReceitasAsync`.
- **Resultado Esperado:** `HttpRequestException` e lancada.
- **Criterios de Sucesso:** `HttpRequestException` e lancada.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Cliente TCE desserializa empenhos por orgao

- **ID do Caso de Teste:** CT-096
- **Nome:** Cliente TCE desserializa empenhos por orgao
- **Objetivo:** Validar o cenario: Cliente TCE desserializa empenhos por orgao.
- **Pre-condicoes:** handler retorna HTTP 200.
- **Entradas:**
  1. envelope JSON com um empenho.
- **Passos para Execucao:**
  1. chamar `GetEmpenhosByOrgaoAsync(2025, "001")`.
- **Resultado Esperado:** item contem numero, valor e natureza de despesa esperados.
- **Criterios de Sucesso:** item contem numero, valor e natureza de despesa esperados.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Cliente TCE retorna vazio em erro de empenhos

- **ID do Caso de Teste:** CT-097
- **Nome:** Cliente TCE retorna vazio em erro de empenhos
- **Objetivo:** Validar o cenario: Cliente TCE retorna vazio em erro de empenhos.
- **Pre-condicoes:** handler retorna HTTP 404.
- **Entradas:**
  1. orgao inexistente `999`.
- **Passos para Execucao:**
  1. chamar metodo de empenhos por orgao.
- **Resultado Esperado:** lista vazia, sem excecao.
- **Criterios de Sucesso:** lista vazia, sem excecao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Cliente TCE desserializa contratos

- **ID do Caso de Teste:** CT-098
- **Nome:** Cliente TCE desserializa contratos
- **Objetivo:** Validar o cenario: Cliente TCE desserializa contratos.
- **Pre-condicoes:** handler retorna HTTP 200.
- **Entradas:**
  1. envelope JSON com um contrato.
- **Passos para Execucao:**
  1. chamar `GetContratosAsync(2025)`.
- **Resultado Esperado:** contrato retornado contem numero e valor esperados.
- **Criterios de Sucesso:** contrato retornado contem numero e valor esperados.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Cliente TCE retorna vazio em erro de contratos

- **ID do Caso de Teste:** CT-099
- **Nome:** Cliente TCE retorna vazio em erro de contratos
- **Objetivo:** Validar o cenario: Cliente TCE retorna vazio em erro de contratos.
- **Pre-condicoes:** handler retorna HTTP 503.
- **Entradas:**
  1. ano 2025.
- **Passos para Execucao:**
  1. chamar `GetContratosAsync`.
- **Resultado Esperado:** lista vazia, sem excecao.
- **Criterios de Sucesso:** lista vazia, sem excecao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. Cliente TCE retorna vazio em erro de orcamento

- **ID do Caso de Teste:** CT-100
- **Nome:** Cliente TCE retorna vazio em erro de orcamento
- **Objetivo:** Validar o cenario: Cliente TCE retorna vazio em erro de orcamento.
- **Pre-condicoes:** handler retorna HTTP 502.
- **Entradas:**
  1. ano 2025.
- **Passos para Execucao:**
  1. chamar `GetOrcamentoAsync`.
- **Resultado Esperado:** lista vazia, sem excecao.
- **Criterios de Sucesso:** lista vazia, sem excecao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Middlewares - GlobalExceptionMiddleware

**Arquivo:** `tests/TransparenciaPE.UnitTests/Middlewares/GlobalExceptionMiddlewareTests.cs`

#### 1. Middleware chama proximo delegate sem excecao

- **ID do Caso de Teste:** CT-101
- **Nome:** Middleware chama proximo delegate sem excecao
- **Objetivo:** Validar o cenario: Middleware chama proximo delegate sem excecao.
- **Pre-condicoes:** `RequestDelegate` apenas marca flag de chamada.
- **Entradas:**
  1. `DefaultHttpContext` com corpo gravavel.
- **Passos para Execucao:**
  1. executar `InvokeAsync`.
- **Resultado Esperado:** proximo delegate e chamado e status permanece `200`.
- **Criterios de Sucesso:** proximo delegate e chamado e status permanece `200`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Middleware mapeia excecoes para status HTTP

- **ID do Caso de Teste:** CT-102
- **Nome:** Middleware mapeia excecoes para status HTTP
- **Objetivo:** Validar o cenario: Middleware mapeia excecoes para status HTTP.
- **Pre-condicoes:** delegate configurado para lancar excecao.
- **Entradas:**
  1. `ArgumentException`, `NotFoundException` e `InvalidOperationException`.
- **Passos para Execucao:**
  1. executar middleware para cada excecao.
- **Resultado Esperado:** status `400`, `404` e `500`, respectivamente.
- **Criterios de Sucesso:** status `400`, `404` e `500`, respectivamente.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Middleware retorna erro JSON estruturado

- **ID do Caso de Teste:** CT-103
- **Nome:** Middleware retorna erro JSON estruturado
- **Objetivo:** Validar o cenario: Middleware retorna erro JSON estruturado.
- **Pre-condicoes:** delegate lanca `ArgumentException`.
- **Entradas:**
  1. contexto HTTP com `MemoryStream`.
- **Passos para Execucao:**
  1. executar middleware, ler corpo e parsear JSON.
- **Resultado Esperado:** resposta `application/json` com `statusCode`, `message` e `timestamp`.
- **Criterios de Sucesso:** resposta `application/json` com `statusCode`, `message` e `timestamp`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Conventions - UnitTests

**Arquivo:** `tests/TransparenciaPE.UnitTests/Conventions/TestNamingConventionTests.cs`

#### 1. Convencao valida nomes dos testes unitarios

- **ID do Caso de Teste:** CT-104
- **Nome:** Convencao valida nomes dos testes unitarios
- **Objetivo:** Validar o cenario: Convencao valida nomes dos testes unitarios.
- **Pre-condicoes:** assembly de testes unitarios carregado.
- **Entradas:**
  1. metodos com `[Fact]` ou `[Theory]`.
- **Passos para Execucao:**
  1. varrer metodos publicos declarados no assembly.
- **Resultado Esperado:** todos seguem o padrao `Assunto_Comportamento_WhenCondicao`.
- **Criterios de Sucesso:** todos seguem o padrao `Assunto_Comportamento_WhenCondicao`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de IntegrationTests - API Endpoints

**Arquivo:** `tests/TransparenciaPE.IntegrationTests/ApiEndpointsTests.cs`

#### 1. Endpoint de resumo retorna HTTP 200

- **ID do Caso de Teste:** CT-105
- **Nome:** Endpoint de resumo retorna HTTP 200
- **Objetivo:** Validar o cenario: Endpoint de resumo retorna HTTP 200.
- **Pre-condicoes:** `CustomWebApplicationFactory` sobe API em ambiente `Testing`.
- **Entradas:**
  1. `GET /api/v1/dashboard/resumo?ano=2025`.
- **Passos para Execucao:**
  1. executar requisicao via `HttpClient`.
- **Resultado Esperado:** status `200 OK`.
- **Criterios de Sucesso:** status `200 OK`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 2. Endpoint de resumo retorna JSON

- **ID do Caso de Teste:** CT-106
- **Nome:** Endpoint de resumo retorna JSON
- **Objetivo:** Validar o cenario: Endpoint de resumo retorna JSON.
- **Pre-condicoes:** API de teste ativa.
- **Entradas:**
  1. mesma rota de resumo.
- **Passos para Execucao:**
  1. executar requisicao e ler header `Content-Type`.
- **Resultado Esperado:** media type `application/json`.
- **Criterios de Sucesso:** media type `application/json`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 3. Endpoint de resumo retorna payload de KPIs

- **ID do Caso de Teste:** CT-107
- **Nome:** Endpoint de resumo retorna payload de KPIs
- **Objetivo:** Validar o cenario: Endpoint de resumo retorna payload de KPIs.
- **Pre-condicoes:** `FakeDashboardService` retorna dados deterministas.
- **Entradas:**
  1. ano 2025.
- **Passos para Execucao:**
  1. desserializar resposta para `DashboardResumoDto`.
- **Resultado Esperado:** `TotalEmpenhado=1000` e `PercentualExecutado=60`.
- **Criterios de Sucesso:** `TotalEmpenhado=1000` e `PercentualExecutado=60`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 4. Endpoint de evolucao retorna HTTP 200

- **ID do Caso de Teste:** CT-108
- **Nome:** Endpoint de evolucao retorna HTTP 200
- **Objetivo:** Validar o cenario: Endpoint de evolucao retorna HTTP 200.
- **Pre-condicoes:** API de teste ativa.
- **Entradas:**
  1. `GET /api/v1/dashboard/evolucao?codigoOrgao=001&ano=2025`.
- **Passos para Execucao:**
  1. executar requisicao.
- **Resultado Esperado:** status `200 OK`.
- **Criterios de Sucesso:** status `200 OK`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 5. Endpoint de evolucao retorna drill-down

- **ID do Caso de Teste:** CT-109
- **Nome:** Endpoint de evolucao retorna drill-down
- **Objetivo:** Validar o cenario: Endpoint de evolucao retorna drill-down.
- **Pre-condicoes:** `FakeDashboardService` retorna um item de drill-down.
- **Entradas:**
  1. `codigoOrgao=001`.
- **Passos para Execucao:**
  1. desserializar resposta para `DrillDownDto`.
- **Resultado Esperado:** `CodigoOrgao="001"` e `Itens` contem um item.
- **Criterios de Sucesso:** `CodigoOrgao="001"` e `Itens` contem um item.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 6. Pesquisa global retorna 400 para termo vazio

- **ID do Caso de Teste:** CT-110
- **Nome:** Pesquisa global retorna 400 para termo vazio
- **Objetivo:** Validar o cenario: Pesquisa global retorna 400 para termo vazio.
- **Pre-condicoes:** `FakePesquisaService` rejeita termo vazio.
- **Entradas:**
  1. `GET /api/v1/pesquisa/global?termo=`.
- **Passos para Execucao:**
  1. executar requisicao.
- **Resultado Esperado:** status `400 BadRequest`.
- **Criterios de Sucesso:** status `400 BadRequest`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 7. Pesquisa global retorna payload de erro

- **ID do Caso de Teste:** CT-111
- **Nome:** Pesquisa global retorna payload de erro
- **Objetivo:** Validar o cenario: Pesquisa global retorna payload de erro.
- **Pre-condicoes:** rota valida a falha do termo vazio.
- **Entradas:**
  1. resposta de erro da API.
- **Passos para Execucao:**
  1. parsear corpo como JSON e verificar `message` ou `errors`.
- **Resultado Esperado:** payload possui mensagem ou erros de validacao.
- **Criterios de Sucesso:** payload possui mensagem ou erros de validacao.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 8. Exportacao CSV retorna HTTP 200

- **ID do Caso de Teste:** CT-112
- **Nome:** Exportacao CSV retorna HTTP 200
- **Objetivo:** Validar o cenario: Exportacao CSV retorna HTTP 200.
- **Pre-condicoes:** `FakePesquisaService` retorna bytes CSV.
- **Entradas:**
  1. `GET /api/v1/exportar/csv?ano=2025`.
- **Passos para Execucao:**
  1. executar requisicao.
- **Resultado Esperado:** status `200 OK`.
- **Criterios de Sucesso:** status `200 OK`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 9. Exportacao CSV retorna content-type correto

- **ID do Caso de Teste:** CT-113
- **Nome:** Exportacao CSV retorna content-type correto
- **Objetivo:** Validar o cenario: Exportacao CSV retorna content-type correto.
- **Pre-condicoes:** API de teste ativa.
- **Entradas:**
  1. rota de exportacao CSV.
- **Passos para Execucao:**
  1. ler `Content-Type`.
- **Resultado Esperado:** media type `text/csv`.
- **Criterios de Sucesso:** media type `text/csv`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 10. Exportacao CSV retorna cabecalho

- **ID do Caso de Teste:** CT-114
- **Nome:** Exportacao CSV retorna cabecalho
- **Objetivo:** Validar o cenario: Exportacao CSV retorna cabecalho.
- **Pre-condicoes:** servico fake retorna CSV deterministico.
- **Entradas:**
  1. ano 2025.
- **Passos para Execucao:**
  1. ler conteudo como string.
- **Resultado Esperado:** conteudo inicia com `NumeroEmpenho;Orgao;Valor`.
- **Criterios de Sucesso:** conteudo inicia com `NumeroEmpenho;Orgao;Valor`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

#### 11. Exportacao CSV retorna nome de arquivo

- **ID do Caso de Teste:** CT-115
- **Nome:** Exportacao CSV retorna nome de arquivo
- **Objetivo:** Validar o cenario: Exportacao CSV retorna nome de arquivo.
- **Pre-condicoes:** resposta de arquivo gerada pelo controller.
- **Entradas:**
  1. chamada com ano 2025.
- **Passos para Execucao:**
  1. ler header `Content-Disposition`.
- **Resultado Esperado:** nome de arquivo contem `.csv`.
- **Criterios de Sucesso:** nome de arquivo contem `.csv`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

### Modulo de Conventions - IntegrationTests

**Arquivo:** `tests/TransparenciaPE.IntegrationTests/Conventions/TestNamingConventionTests.cs`

#### 1. Convencao valida nomes dos testes de integracao

- **ID do Caso de Teste:** CT-116
- **Nome:** Convencao valida nomes dos testes de integracao
- **Objetivo:** Validar o cenario: Convencao valida nomes dos testes de integracao.
- **Pre-condicoes:** assembly de integracao carregado.
- **Entradas:**
  1. metodos com `[Fact]` ou `[Theory]`.
- **Passos para Execucao:**
  1. varrer metodos publicos declarados no assembly.
- **Resultado Esperado:** todos seguem o padrao `Assunto_Comportamento_WhenCondicao`.
- **Criterios de Sucesso:** todos seguem o padrao `Assunto_Comportamento_WhenCondicao`.
- **Criterios de Falha:** O caso falha se o resultado observado divergir do esperado, se uma chamada obrigatoria nao ocorrer ou se ocorrer uma excecao inesperada.

## Observacoes de cobertura

- Os testes unitarios isolam dependencias com Moq sempre que ha repositorios, clientes HTTP, servicos ou loggers.
- Os testes de repositorio e contexto usam banco em memoria para validar comportamento de persistencia e mapeamento.
- Os testes de integracao usam `CustomWebApplicationFactory` com servicos fake para validar contratos HTTP da API sem depender de banco ou APIs externas.
- O objetivo dos CTs e manter um comportamento por metodo sempre que possivel; cenarios amplos foram quebrados em testes menores para facilitar diagnostico.
