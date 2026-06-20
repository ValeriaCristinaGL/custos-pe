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

### Helpers - CnpjHelper

**Arquivo:** `tests/TransparenciaPE.UnitTests/Helpers/CnpjHelperTests.cs`

#### CT-001 - Sanitize remove pontuacao quando a entrada contem mascara

- **Metodo:** `Sanitize_ReturnsDigitsOnly_WhenInputHasPunctuation`
- **Pre-condicoes:** Nenhuma; helper estatico sem dependencias externas.
- **Dados:** `"11.222.333/0001-81"` e `"00.000.000/0000-00"`.
- **Passos:** executar `CnpjHelper.Sanitize(input)` para cada entrada.
- **Resultado esperado:** retorno contem apenas digitos, preservando a ordem numerica original.

#### CT-002 - Sanitize preserva CNPJ que ja possui apenas digitos

- **Metodo:** `Sanitize_ReturnsSameValue_WhenInputHasOnlyDigits`
- **Pre-condicoes:** Nenhuma.
- **Dados:** `"11222333000181"`.
- **Passos:** chamar `CnpjHelper.Sanitize` com o valor ja sanitizado.
- **Resultado esperado:** retorno exatamente igual a entrada, sem inclusao ou remocao indevida de digitos.

#### CT-003 - Sanitize remove espacos externos e pontuacao

- **Metodo:** `Sanitize_ReturnsDigitsOnly_WhenInputHasSurroundingWhitespace`
- **Pre-condicoes:** Nenhuma.
- **Dados:** `"  11.222.333/0001-81  "`.
- **Passos:** executar a sanitizacao sobre a string com espacos e mascara.
- **Resultado esperado:** retorno `"11222333000181"`.

#### CT-004 - Sanitize retorna vazio para entrada nula, vazia ou em branco

- **Metodo:** `Sanitize_ReturnsEmpty_WhenInputIsNullOrWhitespace`
- **Pre-condicoes:** Nenhuma.
- **Dados:** `null`, `""` e `"   "`.
- **Passos:** chamar `CnpjHelper.Sanitize` para cada valor.
- **Resultado esperado:** retorno `string.Empty`, sem lancar excecao.

#### CT-005 - IsValid aceita CNPJs validos

- **Metodo:** `IsValid_ReturnsTrue_WhenCnpjIsValid`
- **Pre-condicoes:** Nenhuma.
- **Dados:** CNPJ valido com e sem mascara.
- **Passos:** chamar `CnpjHelper.IsValid(cnpj)`.
- **Resultado esperado:** retorno `true` para as duas representacoes do mesmo CNPJ valido.

#### CT-006 - IsValid rejeita CNPJs invalidos

- **Metodo:** `IsValid_ReturnsFalse_WhenCnpjIsInvalid`
- **Pre-condicoes:** Nenhuma.
- **Dados:** CNPJs com todos os digitos iguais, tamanho incorreto, vazio, nulo e longo demais.
- **Passos:** chamar `CnpjHelper.IsValid(cnpj)` para cada entrada.
- **Resultado esperado:** retorno `false` em todos os casos invalidos.

### Helpers - McaspMapper

**Arquivo:** `tests/TransparenciaPE.UnitTests/Helpers/McaspMapperTests.cs`

#### CT-007 - Classificacao MCASP identifica despesas de pessoal

- **Metodo:** `MapToClassificacao_ReturnsPessoalEncargos_WhenNaturezaStartsWith31`
- **Pre-condicoes:** Nenhuma.
- **Dados:** naturezas iniciadas por `3.1`.
- **Passos:** chamar `McaspMapper.MapToClassificacao(natureza, "")`.
- **Resultado esperado:** retorno `"Pessoal e Encargos Sociais"`.

#### CT-008 - Classificacao MCASP identifica custeio

- **Metodo:** `MapToClassificacao_ReturnsCusteio_WhenNaturezaStartsWith33`
- **Pre-condicoes:** Nenhuma.
- **Dados:** naturezas iniciadas por `3.3`.
- **Passos:** mapear cada natureza informada.
- **Resultado esperado:** retorno `"Custeio"`.

#### CT-009 - Classificacao MCASP identifica investimentos

- **Metodo:** `MapToClassificacao_ReturnsInvestimentos_WhenNaturezaStartsWith44`
- **Pre-condicoes:** Nenhuma.
- **Dados:** naturezas iniciadas por `4.4`.
- **Passos:** mapear cada natureza informada.
- **Resultado esperado:** retorno `"Investimentos"`.

#### CT-010 - Classificacao MCASP trata prefixos desconhecidos como outros

- **Metodo:** `MapToClassificacao_ReturnsOutros_WhenNaturezaPrefixIsUnknown`
- **Pre-condicoes:** Nenhuma.
- **Dados:** prefixos `3.2`, `4.5`, `4.6`, `5.0` e `10.0`.
- **Passos:** chamar o mapper para cada prefixo nao mapeado.
- **Resultado esperado:** retorno `"Outros"` para todos os casos.

#### CT-011 - Classificacao MCASP trata natureza em branco como outros

- **Metodo:** `MapToClassificacao_ReturnsOutros_WhenNaturezaIsBlank`
- **Pre-condicoes:** Nenhuma.
- **Dados:** `""` e `"   "`.
- **Passos:** chamar o mapper com valor vazio ou apenas espacos.
- **Resultado esperado:** retorno `"Outros"` sem erro de parsing.

#### CT-012 - Classificacao MCASP trata natureza nula como outros

- **Metodo:** `MapToClassificacao_ReturnsOutros_WhenNaturezaIsNull`
- **Pre-condicoes:** Nenhuma.
- **Dados:** `null`.
- **Passos:** chamar o mapper com `null!`.
- **Resultado esperado:** retorno `"Outros"` sem `NullReferenceException`.

### Entities - Orcamento

**Arquivo:** `tests/TransparenciaPE.UnitTests/Entities/OrcamentoTests.cs`

#### CT-013 - Orcamento armazena campos orcamentarios

- **Metodo:** `Orcamento_AssignsBudgetFields_WhenInitialized`
- **Pre-condicoes:** Nenhuma.
- **Dados:** `Ano=2026`, `DotacaoInicial=450000`, `DotacaoAtualizada=500000`.
- **Passos:** instanciar `Orcamento` com os valores.
- **Resultado esperado:** propriedades retornam exatamente os valores informados.

#### CT-014 - Orcamento armazena identificador do orgao

- **Metodo:** `Orcamento_AssignsOrgaoGovernoId_WhenInitialized`
- **Pre-condicoes:** `Guid` gerado para representar o orgao de governo.
- **Dados:** `OrgaoGovernoId=<Guid>`.
- **Passos:** instanciar `Orcamento` atribuindo o identificador.
- **Resultado esperado:** `OrgaoGovernoId` permanece igual ao `Guid` atribuido.

#### CT-015 - Orcamento gera identificador proprio

- **Metodo:** `Orcamento_GeneratesId_WhenCreated`
- **Pre-condicoes:** Nenhuma.
- **Dados:** nova instancia sem propriedades informadas.
- **Passos:** criar `new Orcamento()`.
- **Resultado esperado:** `Id` nao e `Guid.Empty`.

### Entities - OrgaoGoverno

**Arquivo:** `tests/TransparenciaPE.UnitTests/Entities/OrgaoGovernoTests.cs`

#### CT-016 - OrgaoGoverno gera IDs unicos

- **Metodo:** `OrgaoGoverno_GeneratesUniqueId_WhenCreated`
- **Pre-condicoes:** Nenhuma.
- **Dados:** duas instancias independentes de `OrgaoGoverno`.
- **Passos:** criar as duas instancias e comparar seus IDs.
- **Resultado esperado:** ambos os IDs sao nao vazios e diferentes entre si.

#### CT-017 - OrgaoGoverno inicializa colecoes de navegacao

- **Metodo:** `OrgaoGoverno_InitializesCollections_WhenCreated`
- **Pre-condicoes:** Nenhuma.
- **Dados:** nova instancia vazia.
- **Passos:** acessar `Empenhos` e `Contratos`.
- **Resultado esperado:** colecoes sao nao nulas e vazias.

#### CT-018 - OrgaoGoverno define CreatedAt na criacao

- **Metodo:** `OrgaoGoverno_SetsCreatedAt_WhenCreated`
- **Pre-condicoes:** janela de tempo registrada antes e depois da instanciacao.
- **Dados:** nova instancia vazia.
- **Passos:** criar o orgao e validar `CreatedAt` dentro da janela.
- **Resultado esperado:** `CreatedAt` e recente e `UpdatedAt` inicia nulo.

#### CT-019 - OrgaoGoverno armazena dados de identificacao

- **Metodo:** `OrgaoGoverno_AssignsIdentificationFields_WhenInitialized`
- **Pre-condicoes:** Nenhuma.
- **Dados:** codigo, nome, sigla e tipo.
- **Passos:** criar a entidade com os valores de identificacao.
- **Resultado esperado:** todas as propriedades retornam os valores atribuidos.

#### CT-020 - OrgaoGoverno armazena dados de capacidade

- **Metodo:** `OrgaoGoverno_AssignsCapacityFields_WhenInitialized`
- **Pre-condicoes:** Nenhuma.
- **Dados:** `TotalServidores=1500` e `OrcamentoAtual=5000000`.
- **Passos:** criar a entidade com esses valores.
- **Resultado esperado:** campos numericos sao preservados sem perda.

### Entities - Receita

**Arquivo:** `tests/TransparenciaPE.UnitTests/Entities/ReceitaTests.cs`

#### CT-021 - Receita armazena campos contabeis

- **Metodo:** `Receita_AssignsAccountingFields_WhenInitialized`
- **Pre-condicoes:** Nenhuma.
- **Dados:** valor, mes, ano e origem.
- **Passos:** instanciar `Receita` com os dados contabeis.
- **Resultado esperado:** propriedades retornam exatamente os valores atribuidos.

#### CT-022 - Receita armazena identificador do orgao

- **Metodo:** `Receita_AssignsOrgaoGovernoId_WhenInitialized`
- **Pre-condicoes:** `Guid` valido para orgao.
- **Dados:** `OrgaoGovernoId=<Guid>`.
- **Passos:** atribuir o identificador na instanciacao.
- **Resultado esperado:** `OrgaoGovernoId` e igual ao valor recebido.

#### CT-023 - Receita gera identificador proprio

- **Metodo:** `Receita_GeneratesId_WhenCreated`
- **Pre-condicoes:** Nenhuma.
- **Dados:** nova instancia vazia.
- **Passos:** criar `new Receita()`.
- **Resultado esperado:** `Id` nao e vazio.

### Services - DashboardService

**Arquivo:** `tests/TransparenciaPE.UnitTests/Services/DashboardServiceTests.cs`

#### CT-024 - Resumo retorna KPIs quando ha dados

- **Metodo:** `GetResumoAsync_ReturnsKPIs_WhenDataExists`
- **Pre-condicoes:** `IDashboardQueryService` mockado com totais de empenhado, liquidado, pago, empenhos e contratos.
- **Dados:** `TotalEmpenhado=1000000`, `TotalPago=600000`.
- **Passos:** chamar `_sut.GetResumoAsync()` e inspecionar o DTO.
- **Resultado esperado:** totais sao mapeados e `PercentualExecutado=60`.

#### CT-025 - Resumo evita divisao por zero

- **Metodo:** `GetResumoAsync_ReturnsZeroPercentualExecutado_WhenTotalEmpenhadoIsZero`
- **Pre-condicoes:** query retorna `TotalEmpenhado=0`.
- **Dados:** totais zerados.
- **Passos:** chamar `_sut.GetResumoAsync()`.
- **Resultado esperado:** `PercentualExecutado` retorna `0`.

#### CT-026 - Resumo repassa filtro de ano

- **Metodo:** `GetResumoAsync_PassesYearParameter_WhenYearIsProvided`
- **Pre-condicoes:** mock configurado para receber `2025`.
- **Dados:** `ano=2025`.
- **Passos:** chamar `_sut.GetResumoAsync(2025)`.
- **Resultado esperado:** `GetResumoAsync(2025)` do query service e chamado uma vez.

#### CT-027 - Comparativo retorna ano solicitado

- **Metodo:** `GetComparativoOrgaosAsync_ReturnsAno_WhenYearIsProvided`
- **Pre-condicoes:** query retorna lista vazia.
- **Dados:** `ano=2025`.
- **Passos:** chamar `_sut.GetComparativoOrgaosAsync(2025)`.
- **Resultado esperado:** DTO retorna `Ano=2025`.

#### CT-028 - Comparativo retorna lista de orgaos

- **Metodo:** `GetComparativoOrgaosAsync_ReturnsOrgaos_WhenDataExists`
- **Pre-condicoes:** query retorna dois orgaos com valores de execucao.
- **Dados:** orgaos `001` e `002`.
- **Passos:** chamar o comparativo e contar os itens.
- **Resultado esperado:** `Orgaos` possui dois itens mapeados.

#### CT-029 - Drill-down retorna codigo do orgao

- **Metodo:** `GetDrillDownAsync_ReturnsCodigoOrgao_WhenCodigoOrgaoIsProvided`
- **Pre-condicoes:** query retorna lista vazia.
- **Dados:** `codigoOrgao="001"`.
- **Passos:** chamar `_sut.GetDrillDownAsync("001")`.
- **Resultado esperado:** DTO retorna `CodigoOrgao="001"`.

#### CT-030 - Drill-down retorna itens hierarquicos

- **Metodo:** `GetDrillDownAsync_ReturnsItens_WhenDataExists`
- **Pre-condicoes:** query retorna um item de classificacao MCASP.
- **Dados:** classificacao `3.3.90.30`, descricao e total.
- **Passos:** chamar o drill-down e verificar a colecao.
- **Resultado esperado:** `Itens` contem exatamente um item.

### Services - PesquisaService

**Arquivo:** `tests/TransparenciaPE.UnitTests/Services/PesquisaServiceTests.cs`

#### CT-031 - Pesquisa global usa fluxo de CNPJ

- **Metodo:** `PesquisaGlobalAsync_SearchesContratosByCnpj_WhenTermIsCnpj`
- **Pre-condicoes:** repositorio de contratos mockado para CNPJ sanitizado.
- **Dados:** CNPJ com e sem mascara.
- **Passos:** chamar `PesquisaGlobalAsync(termo)`.
- **Resultado esperado:** busca por CNPJ e executada uma vez e busca por fornecedor nao e executada.

#### CT-032 - Pesquisa global usa fluxo textual

- **Metodo:** `PesquisaGlobalAsync_SearchesContratosByFornecedor_WhenTermIsText`
- **Pre-condicoes:** repositorio de contratos mockado para fornecedor.
- **Dados:** `termo="Empresa ABC"`.
- **Passos:** executar pesquisa global com termo textual.
- **Resultado esperado:** `SearchByFornecedorAsync("Empresa ABC")` e chamado uma vez e termo buscado e preservado.

#### CT-033 - Pesquisa global rejeita termo vazio

- **Metodo:** `PesquisaGlobalAsync_ThrowsArgumentException_WhenTermIsEmpty`
- **Pre-condicoes:** Nenhuma chamada aos repositorios deve ser necessaria.
- **Dados:** `""`, `null` e `"   "`.
- **Passos:** chamar `PesquisaGlobalAsync` com cada valor invalido.
- **Resultado esperado:** `ArgumentException` e lancada.

#### CT-034 - Exportacao CSV retorna dados de empenho

- **Metodo:** `ExportarCsvAsync_ReturnsCsvWithEmpenhoData_WhenDataExists`
- **Pre-condicoes:** repositorio de empenhos retorna um empenho com orgao e credor.
- **Dados:** `NumeroEmpenho="EMP-001"` e `Credor="Empresa A"`.
- **Passos:** chamar `ExportarCsvAsync()` e decodificar bytes UTF-8.
- **Resultado esperado:** CSV contem numero do empenho e fornecedor.

#### CT-035 - Exportacao CSV retorna apenas cabecalho sem dados

- **Metodo:** `ExportarCsvAsync_ReturnsHeaderOnly_WhenNoDataExists`
- **Pre-condicoes:** repositorio retorna colecao vazia.
- **Dados:** nenhum empenho.
- **Passos:** chamar `ExportarCsvAsync()` e separar linhas.
- **Resultado esperado:** CSV contem somente a linha de cabecalho.

#### CT-036 - Exportacao CSV aplica filtro de ano

- **Metodo:** `ExportarCsvAsync_UsesYearFilter_WhenYearIsProvided`
- **Pre-condicoes:** callback captura a expressao enviada ao repositorio.
- **Dados:** filtro `ano=2025`.
- **Passos:** compilar o predicado capturado e testar empenhos de 2025 e 2024.
- **Resultado esperado:** predicado aceita 2025 e rejeita 2024.

### Services - DataSyncService

**Arquivo:** `tests/TransparenciaPE.UnitTests/Services/DataSyncServiceTests.cs`

#### CT-037 - Sincronizacao insere empenho novo

- **Metodo:** `SyncEmpenhosAsync_AddsEmpenho_WhenEmpenhoDoesNotExist`
- **Pre-condicoes:** existe orgao base e API externa retorna empenho inexistente.
- **Dados:** `NumeroEmpenho="EMP-001"`, ano 2025 e CNPJ com mascara.
- **Passos:** chamar `SyncEmpenhosAsync(2025)`.
- **Resultado esperado:** empenho e adicionado com CNPJ sanitizado, count retorna `1` e commit e chamado.

#### CT-038 - Sincronizacao atualiza empenho existente

- **Metodo:** `SyncEmpenhosAsync_UpdatesEmpenho_WhenEmpenhoExists`
- **Pre-condicoes:** repositorio retorna empenho ja existente.
- **Dados:** valor antigo `30000` e valor externo `50000`.
- **Passos:** executar sincronizacao do ano.
- **Resultado esperado:** valor do empenho existente e atualizado e nenhum novo empenho e inserido.

#### CT-039 - Sincronizacao sanitiza CNPJ ao adicionar empenho

- **Metodo:** `SyncEmpenhosAsync_SanitizesCnpj_WhenAddingEmpenho`
- **Pre-condicoes:** API retorna empenho novo com CNPJ pontuado.
- **Dados:** `"11.222.333/0001-81"`.
- **Passos:** sincronizar empenhos.
- **Resultado esperado:** `AddAsync` recebe entidade com `CnpjCredor="11222333000181"`.

#### CT-040 - SyncAll retorna timestamp de sincronizacao

- **Metodo:** `SyncAllAsync_ReturnsSyncedAt_WhenSyncCompletes`
- **Pre-condicoes:** APIs retornam listas vazias.
- **Dados:** `ano=2025`.
- **Passos:** chamar `SyncAllAsync(2025)`.
- **Resultado esperado:** `SyncedAt` e preenchido com data/hora menor ou igual ao momento atual.

#### CT-041 - Sincronizacao de empenhos para quando nao ha orgaos

- **Metodo:** `SyncEmpenhosAsync_ReturnsZero_WhenNoOrgaosExist`
- **Pre-condicoes:** repositorio de orgaos retorna lista vazia.
- **Dados:** `ano=2025`.
- **Passos:** chamar `SyncEmpenhosAsync`.
- **Resultado esperado:** retorna `0`, nao consulta API externa e nao realiza commit.

#### CT-042 - Sincronizacao retorna zero quando API nao traz empenhos

- **Metodo:** `SyncEmpenhosAsync_ReturnsZero_WhenApiReturnsEmpty`
- **Pre-condicoes:** existe orgao base, mas API retorna lista vazia.
- **Dados:** orgao `001`.
- **Passos:** chamar sincronizacao.
- **Resultado esperado:** retorna `0`, nao adiciona empenhos e realiza commit da operacao.

#### CT-043 - Sincronizacao insere contrato novo

- **Metodo:** `SyncContratosAsync_AddsContrato_WhenContratoDoesNotExist`
- **Pre-condicoes:** API externa retorna contrato inexistente.
- **Dados:** contrato `CT-2025-001` com CNPJ pontuado.
- **Passos:** chamar `SyncContratosAsync(2025)`.
- **Resultado esperado:** contrato e adicionado com CNPJ sanitizado, count `1` e commit chamado.

#### CT-044 - SyncAll agrega contadores processados

- **Metodo:** `SyncAllAsync_ReturnsProcessedCounts_WhenEmpenhosAndContratosExist`
- **Pre-condicoes:** API retorna um empenho novo e um contrato novo.
- **Dados:** `EMP-X` e `CT-X`.
- **Passos:** executar `SyncAllAsync(2025)`.
- **Resultado esperado:** resultado retorna `EmpenhosProcessados=1` e `ContratosProcessados=1`.

### Controllers - DashboardController

**Arquivo:** `tests/TransparenciaPE.UnitTests/Controllers/DashboardControllerTests.cs`

#### CT-045 - GetResumo retorna Ok

- **Metodo:** `GetResumo_ReturnsOkResult_WhenServiceReturnsResumo`
- **Pre-condicoes:** `IDashboardService` retorna `DashboardResumoDto`.
- **Dados:** chamada sem ano.
- **Passos:** executar `_sut.GetResumo(null)`.
- **Resultado esperado:** resultado HTTP e `OkObjectResult`.

#### CT-046 - GetResumo retorna DTO esperado

- **Metodo:** `GetResumo_ReturnsDashboardResumoDto_WhenServiceReturnsResumo`
- **Pre-condicoes:** servico retorna resumo vazio valido.
- **Dados:** chamada sem ano.
- **Passos:** executar controller e inspecionar `Value`.
- **Resultado esperado:** corpo e do tipo `DashboardResumoDto`.

#### CT-047 - GetResumo propaga total empenhado

- **Metodo:** `GetResumo_ReturnsTotalEmpenhado_WhenServiceReturnsResumo`
- **Pre-condicoes:** servico retorna `TotalEmpenhado=1000000`.
- **Dados:** chamada sem ano.
- **Passos:** executar controller e converter corpo para DTO.
- **Resultado esperado:** DTO retornado contem `TotalEmpenhado=1000000`.

#### CT-048 - GetResumo chama servico com ano nulo

- **Metodo:** `GetResumo_InvokesServiceWithNullYear_WhenAnoIsNull`
- **Pre-condicoes:** mock configurado para `GetResumoAsync(null)`.
- **Dados:** `ano=null`.
- **Passos:** executar endpoint do controller.
- **Resultado esperado:** servico e chamado uma vez com `null`.

#### CT-049 - GetResumo chama servico com ano informado

- **Metodo:** `GetResumo_InvokesServiceWithProvidedYear_WhenAnoIsProvided`
- **Pre-condicoes:** mock configurado para `2025`.
- **Dados:** `ano=2025`.
- **Passos:** executar endpoint do controller.
- **Resultado esperado:** servico e chamado uma vez com `2025`.

#### CT-050 - GetComparativo retorna Ok

- **Metodo:** `GetComparativo_ReturnsOkResult_WhenServiceReturnsComparativo`
- **Pre-condicoes:** servico retorna `ComparativoOrgaosDto`.
- **Dados:** `ano=2025`.
- **Passos:** chamar `_sut.GetComparativo(2025)`.
- **Resultado esperado:** resultado HTTP e `OkObjectResult`.

#### CT-051 - GetComparativo retorna DTO esperado

- **Metodo:** `GetComparativo_ReturnsComparativoOrgaosDto_WhenServiceReturnsComparativo`
- **Pre-condicoes:** servico retorna comparativo valido.
- **Dados:** `ano=2025`.
- **Passos:** executar controller e avaliar corpo.
- **Resultado esperado:** corpo e `ComparativoOrgaosDto`.

#### CT-052 - GetComparativo propaga ano

- **Metodo:** `GetComparativo_ReturnsAno_WhenServiceReturnsComparativo`
- **Pre-condicoes:** servico retorna DTO com `Ano=2025`.
- **Dados:** `ano=2025`.
- **Passos:** executar controller.
- **Resultado esperado:** corpo retorna `Ano=2025`.

#### CT-053 - GetEvolucao retorna Ok

- **Metodo:** `GetEvolucao_ReturnsOkResult_WhenServiceReturnsDrillDown`
- **Pre-condicoes:** servico retorna `DrillDownDto`.
- **Dados:** `codigoOrgao="001"`.
- **Passos:** chamar `_sut.GetEvolucao("001", null)`.
- **Resultado esperado:** resultado HTTP e `OkObjectResult`.

#### CT-054 - GetEvolucao retorna DTO de drill-down

- **Metodo:** `GetEvolucao_ReturnsDrillDownDto_WhenServiceReturnsDrillDown`
- **Pre-condicoes:** servico retorna drill-down valido.
- **Dados:** `codigoOrgao="001"`.
- **Passos:** executar controller e inspecionar corpo.
- **Resultado esperado:** corpo e `DrillDownDto`.

#### CT-055 - GetEvolucao propaga codigo do orgao

- **Metodo:** `GetEvolucao_ReturnsCodigoOrgao_WhenServiceReturnsDrillDown`
- **Pre-condicoes:** servico retorna DTO com `CodigoOrgao="001"`.
- **Dados:** `codigoOrgao="001"`.
- **Passos:** executar controller.
- **Resultado esperado:** corpo preserva `CodigoOrgao="001"`.

#### CT-056 - GetEvolucao chama servico com ano nulo

- **Metodo:** `GetEvolucao_InvokesServiceWithNullYear_WhenAnoIsNull`
- **Pre-condicoes:** mock configurado com codigo `001` e ano nulo.
- **Dados:** `codigoOrgao="001"`, `ano=null`.
- **Passos:** chamar controller.
- **Resultado esperado:** servico recebe exatamente esses parametros.

#### CT-057 - GetEvolucao chama servico com parametros informados

- **Metodo:** `GetEvolucao_InvokesServiceWithProvidedParameters_WhenAnoIsProvided`
- **Pre-condicoes:** mock configurado com codigo `002` e ano `2025`.
- **Dados:** `codigoOrgao="002"`, `ano=2025`.
- **Passos:** chamar controller.
- **Resultado esperado:** servico e chamado uma vez com codigo e ano informados.

### Controllers - PesquisaController

**Arquivo:** `tests/TransparenciaPE.UnitTests/Controllers/PesquisaControllerTests.cs`

#### CT-058 - PesquisaGlobal retorna Ok

- **Metodo:** `PesquisaGlobal_ReturnsOkResult_WhenServiceReturnsResult`
- **Pre-condicoes:** servico retorna `PesquisaResultDto`.
- **Dados:** termo `"Empresa A"`.
- **Passos:** chamar `_sut.PesquisaGlobal("Empresa A")`.
- **Resultado esperado:** resultado HTTP e `OkObjectResult`.

#### CT-059 - PesquisaGlobal retorna DTO esperado

- **Metodo:** `PesquisaGlobal_ReturnsPesquisaResultDto_WhenServiceReturnsResult`
- **Pre-condicoes:** servico retorna resultado valido.
- **Dados:** termo `"Empresa A"`.
- **Passos:** executar controller e inspecionar corpo.
- **Resultado esperado:** corpo e `PesquisaResultDto`.

#### CT-060 - PesquisaGlobal propaga termo buscado

- **Metodo:** `PesquisaGlobal_ReturnsTermoBuscado_WhenServiceReturnsResult`
- **Pre-condicoes:** servico retorna `TermoBuscado="Empresa A"`.
- **Dados:** termo `"Empresa A"`.
- **Passos:** executar controller.
- **Resultado esperado:** corpo preserva o termo buscado.

#### CT-061 - PesquisaGlobal retorna status 400 para termo vazio

- **Metodo:** `PesquisaGlobal_ReturnsBadRequestStatusCode_WhenTermoIsEmpty`
- **Pre-condicoes:** servico lanca `ArgumentException` para termo vazio.
- **Dados:** `termo=""`.
- **Passos:** chamar controller com termo vazio.
- **Resultado esperado:** `BadRequestObjectResult.StatusCode=400`.

#### CT-062 - PesquisaGlobal retorna BadRequestObjectResult

- **Metodo:** `PesquisaGlobal_ReturnsBadRequestObjectResult_WhenTermoIsEmpty`
- **Pre-condicoes:** servico lanca `ArgumentException`.
- **Dados:** `termo=""`.
- **Passos:** chamar controller.
- **Resultado esperado:** `result.Result` e `BadRequestObjectResult`.

#### CT-063 - ExportarCsv retorna arquivo

- **Metodo:** `ExportarCsv_ReturnsFileContentResult_WhenServiceReturnsCsv`
- **Pre-condicoes:** servico retorna bytes CSV.
- **Dados:** chamada sem termo e sem ano.
- **Passos:** chamar `_sut.ExportarCsv(null, null)`.
- **Resultado esperado:** retorno e `FileContentResult`.

#### CT-064 - ExportarCsv define content-type CSV

- **Metodo:** `ExportarCsv_ReturnsCsvContentType_WhenServiceReturnsCsv`
- **Pre-condicoes:** servico retorna bytes CSV.
- **Dados:** CSV simples em UTF-8.
- **Passos:** chamar controller e ler `ContentType`.
- **Resultado esperado:** `ContentType="text/csv"`.

#### CT-065 - ExportarCsv define nome de arquivo

- **Metodo:** `ExportarCsv_ReturnsFileNameWithCsvExtension_WhenServiceReturnsCsv`
- **Pre-condicoes:** servico retorna bytes CSV.
- **Dados:** chamada sem filtros.
- **Passos:** chamar controller e ler `FileDownloadName`.
- **Resultado esperado:** nome de download contem `.csv`.

### Infrastructure - AppDbContext

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/AppDbContextTests.cs`

#### CT-066 - Modelo aplica configuracao de Empenho

- **Metodo:** `Model_AppliesEmpenhoConfiguration_WhenBuilt`
- **Pre-condicoes:** contexto em memoria criado.
- **Dados:** metadados EF da entidade `Empenho`.
- **Passos:** consultar tabela, tamanhos de campos e tipo decimal.
- **Resultado esperado:** tabela `empenhos`, limites esperados e `numeric(18,2)`.

#### CT-067 - Modelo aplica configuracao de Contrato

- **Metodo:** `Model_AppliesContratoConfiguration_WhenBuilt`
- **Pre-condicoes:** contexto em memoria criado.
- **Dados:** metadados EF da entidade `Contrato`.
- **Passos:** verificar tabela e limites de propriedades.
- **Resultado esperado:** tabela `contratos`, numero com tamanho 50 e objeto com tamanho 1000.

#### CT-068 - Modelo aplica configuracao de OrgaoGoverno

- **Metodo:** `Model_AppliesOrgaoGovernoConfiguration_WhenBuilt`
- **Pre-condicoes:** contexto em memoria criado.
- **Dados:** metadados da entidade `OrgaoGoverno`.
- **Passos:** verificar nome da tabela e limites.
- **Resultado esperado:** tabela `orgaos_governo`, codigo ate 20 e nome ate 200.

#### CT-069 - Modelo aplica configuracao de Liquidacao

- **Metodo:** `Model_AppliesLiquidacaoConfiguration_WhenBuilt`
- **Pre-condicoes:** contexto em memoria criado.
- **Dados:** metadados de `Liquidacao`.
- **Passos:** verificar tabela e tamanho do numero.
- **Resultado esperado:** tabela `liquidacoes` e numero com limite 50.

#### CT-070 - Modelo aplica configuracao de Pagamento

- **Metodo:** `Model_AppliesPagamentoConfiguration_WhenBuilt`
- **Pre-condicoes:** contexto em memoria criado.
- **Dados:** metadados de `Pagamento`.
- **Passos:** verificar tabela e tamanho do numero.
- **Resultado esperado:** tabela `pagamentos` e numero com limite 50.

#### CT-071 - SaveChanges define CreatedAt ao adicionar

- **Metodo:** `SaveChangesAsync_SetsCreatedAt_WhenEntityIsAdded`
- **Pre-condicoes:** entidade criada com `CreatedAt` antigo.
- **Dados:** orgao novo.
- **Passos:** adicionar entidade e salvar.
- **Resultado esperado:** `CreatedAt` e atualizado para valor recente e `UpdatedAt` permanece nulo.

#### CT-072 - SaveChanges define UpdatedAt ao modificar

- **Metodo:** `SaveChangesAsync_SetsUpdatedAt_WhenEntityIsModified`
- **Pre-condicoes:** entidade ja persistida.
- **Dados:** alteracao no nome do orgao.
- **Passos:** atualizar entidade e salvar.
- **Resultado esperado:** `UpdatedAt` e preenchido com data recente.

### Infrastructure - FakePEDataClient

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/FakePEDataClientTests.cs`

#### CT-073 - Fake retorna empenhos do ano solicitado

- **Metodo:** `GetEmpenhosAsync_ReturnsSeedData_ForRequestedYear`
- **Pre-condicoes:** cliente fake instanciado.
- **Dados:** `ano=2025`.
- **Passos:** chamar `GetEmpenhosAsync(2025)`.
- **Resultado esperado:** tres empenhos retornados e todos pertencem a 2025.

#### CT-074 - Fake filtra empenhos por orgao

- **Metodo:** `GetEmpenhosByOrgaoAsync_ReturnsFilteredData_ForRequestedOrgao`
- **Pre-condicoes:** cliente fake instanciado.
- **Dados:** `ano=2025`, `codigoOrgao="SES"`.
- **Passos:** chamar metodo de filtro por orgao.
- **Resultado esperado:** retorna um item com `CodigoOrgao="SES"`.

#### CT-075 - Fake retorna contratos do ano solicitado

- **Metodo:** `GetContratosAsync_ReturnsSeedData_ForRequestedYear`
- **Pre-condicoes:** cliente fake instanciado.
- **Dados:** `ano=2025`.
- **Passos:** chamar `GetContratosAsync`.
- **Resultado esperado:** dois contratos com numero iniciado por `CT-2025`.

#### CT-076 - Fake retorna receitas do ano solicitado

- **Metodo:** `GetReceitasAsync_ReturnsSeedData_ForRequestedYear`
- **Pre-condicoes:** cliente fake instanciado.
- **Dados:** `ano=2025`.
- **Passos:** chamar `GetReceitasAsync`.
- **Resultado esperado:** uma receita com ano 2025 e valor esperado.

#### CT-077 - Fake retorna orcamento do ano solicitado

- **Metodo:** `GetOrcamentoAsync_ReturnsSeedData_ForRequestedYear`
- **Pre-condicoes:** cliente fake instanciado.
- **Dados:** `ano=2025`.
- **Passos:** chamar `GetOrcamentoAsync`.
- **Resultado esperado:** um registro com dotacao atualizada esperada.

#### CT-078 - Fake retorna total fixo de servidores

- **Metodo:** `GetTotalServidoresAsync_ReturnsFixedTotal_ForAnyOrgao`
- **Pre-condicoes:** cliente fake instanciado.
- **Dados:** `codigoOrgao="SEE"`.
- **Passos:** chamar `GetTotalServidoresAsync`.
- **Resultado esperado:** retorno `1200`.

### Infrastructure - Repository

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/RepositoryTests.cs`

#### CT-079 - Repositorio adiciona entidade

- **Metodo:** `AddAsync_PersistsEntity_WhenSaved`
- **Pre-condicoes:** contexto em memoria vazio.
- **Dados:** orgao `001`.
- **Passos:** chamar `AddAsync` e `SaveChangesAsync`.
- **Resultado esperado:** entidade passa a existir no `DbSet`.

#### CT-080 - Repositorio busca por ID

- **Metodo:** `GetByIdAsync_ReturnsEntity_WhenEntityExists`
- **Pre-condicoes:** orgao previamente salvo.
- **Dados:** ID do orgao salvo.
- **Passos:** chamar `GetByIdAsync(id)`.
- **Resultado esperado:** entidade retornada possui o mesmo ID.

#### CT-081 - Repositorio lista entidades

- **Metodo:** `GetAllAsync_ReturnsEntities_WhenEntitiesExist`
- **Pre-condicoes:** orgao previamente salvo.
- **Dados:** contexto com um orgao.
- **Passos:** chamar `GetAllAsync()`.
- **Resultado esperado:** lista contem o orgao salvo.

#### CT-082 - Repositorio filtra por predicado

- **Metodo:** `FindAsync_ReturnsMatchingEntities_WhenPredicateMatches`
- **Pre-condicoes:** orgao salvo com codigo `001`.
- **Dados:** predicado `o.Codigo == "001"`.
- **Passos:** chamar `FindAsync`.
- **Resultado esperado:** colecao contem somente o orgao esperado.

#### CT-083 - Repositorio confirma existencia

- **Metodo:** `ExistsAsync_ReturnsTrue_WhenPredicateMatches`
- **Pre-condicoes:** orgao salvo com nome contendo `Educacao`.
- **Dados:** predicado por nome.
- **Passos:** chamar `ExistsAsync`.
- **Resultado esperado:** retorno `true`.

#### CT-084 - Repositorio atualiza entidade

- **Metodo:** `Update_PersistsChanges_WhenSaved`
- **Pre-condicoes:** orgao salvo.
- **Dados:** novo nome `Secretaria Estadual de Educacao`.
- **Passos:** alterar propriedade, chamar `Update` e salvar.
- **Resultado esperado:** busca posterior retorna o nome atualizado.

#### CT-085 - Repositorio remove entidade

- **Metodo:** `Remove_DeletesEntity_WhenSaved`
- **Pre-condicoes:** orgao salvo.
- **Dados:** ID do orgao.
- **Passos:** chamar `Remove`, salvar e verificar existencia.
- **Resultado esperado:** `ExistsAsync` retorna `false`.

#### CT-086 - EmpenhoRepository busca empenho com orgao

- **Metodo:** `GetByNumeroAsync_ReturnsEmpenhoWithOrgao_WhenEmpenhoExists`
- **Pre-condicoes:** empenho e orgao salvos no contexto.
- **Dados:** numero `EMP-001`, ano `2025`.
- **Passos:** chamar `GetByNumeroAsync`.
- **Resultado esperado:** empenho retornado inclui dados do orgao.

#### CT-087 - EmpenhoRepository filtra por ano

- **Metodo:** `GetByAnoAsync_ReturnsOnlyMatchingEmpenhos_WhenYearIsProvided`
- **Pre-condicoes:** existem empenhos de 2025 e 2024.
- **Dados:** `ano=2025`.
- **Passos:** chamar `GetByAnoAsync(2025)`.
- **Resultado esperado:** retorna apenas o empenho de 2025.

#### CT-088 - EmpenhoRepository filtra por orgao

- **Metodo:** `GetByOrgaoAsync_ReturnsOnlyMatchingEmpenhos_WhenOrgaoIsProvided`
- **Pre-condicoes:** existem empenhos de dois orgaos.
- **Dados:** ID do orgao de educacao.
- **Passos:** chamar `GetByOrgaoAsync`.
- **Resultado esperado:** todos os itens retornados pertencem ao orgao informado.

#### CT-089 - ContratoRepository busca contrato com orgao

- **Metodo:** `GetByNumeroAsync_ReturnsContratoWithOrgao_WhenContratoExists`
- **Pre-condicoes:** contrato e orgao salvos.
- **Dados:** numero `CT-001`.
- **Passos:** chamar `GetByNumeroAsync`.
- **Resultado esperado:** contrato retornado inclui fornecedor e orgao.

#### CT-090 - ContratoRepository busca por CNPJ

- **Metodo:** `SearchByCnpjAsync_ReturnsMatchingContratos_WhenCnpjExists`
- **Pre-condicoes:** existem contratos com CNPJs diferentes.
- **Dados:** CNPJ `"11222333000181"`.
- **Passos:** chamar `SearchByCnpjAsync`.
- **Resultado esperado:** retorna somente o contrato com CNPJ correspondente.

### Infrastructure - UnitOfWork

**Arquivo:** `tests/TransparenciaPE.UnitTests/Infrastructure/UnitOfWorkTests.cs`

#### CT-091 - UnitOfWork expoe repositorios configurados

- **Metodo:** `UnitOfWork_ExposesConfiguredRepositories_WhenCreated`
- **Pre-condicoes:** repositorios de empenhos e contratos instanciados.
- **Dados:** contexto em memoria.
- **Passos:** criar `UnitOfWork`.
- **Resultado esperado:** propriedades `Empenhos` e `Contratos` referenciam os repositorios recebidos.

#### CT-092 - UnitOfWork persiste alteracoes pendentes

- **Metodo:** `CommitAsync_PersistsPendingChanges_WhenRepositoryAddsEntity`
- **Pre-condicoes:** entidade adicionada ao contexto.
- **Dados:** orgao novo.
- **Passos:** chamar `CommitAsync`.
- **Resultado esperado:** retorno indica uma alteracao persistida e entidade fica salva.

#### CT-093 - UnitOfWork descarta contexto

- **Metodo:** `Dispose_DisposesDbContext_WhenCalled`
- **Pre-condicoes:** `UnitOfWork` criado com contexto em memoria.
- **Dados:** chamada direta a `Dispose`.
- **Passos:** descartar e tentar acessar o contexto.
- **Resultado esperado:** acesso posterior lanca `ObjectDisposedException`.

### ExternalClients - TcePEDataClient

**Arquivo:** `tests/TransparenciaPE.UnitTests/ExternalClients/TcePEDataClientTests.cs`

#### CT-094 - Cliente TCE desserializa receitas

- **Metodo:** `GetReceitasAsync_ReturnsReceitas_WhenApiReturnsTceJson`
- **Pre-condicoes:** `HttpMessageHandler` mockado retorna HTTP 200.
- **Dados:** envelope JSON do TCE com uma receita.
- **Passos:** chamar `GetReceitasAsync(2026)`.
- **Resultado esperado:** lista contem uma receita com valor e origem esperados.

#### CT-095 - Cliente TCE lanca excecao em erro de receitas

- **Metodo:** `GetReceitasAsync_ThrowsHttpRequestException_WhenApiReturnsServerError`
- **Pre-condicoes:** handler retorna HTTP 500.
- **Dados:** corpo vazio/default.
- **Passos:** chamar `GetReceitasAsync`.
- **Resultado esperado:** `HttpRequestException` e lancada.

#### CT-096 - Cliente TCE desserializa empenhos por orgao

- **Metodo:** `GetEmpenhosByOrgaoAsync_ReturnsEmpenhos_WhenApiReturnsTceJson`
- **Pre-condicoes:** handler retorna HTTP 200.
- **Dados:** envelope JSON com um empenho.
- **Passos:** chamar `GetEmpenhosByOrgaoAsync(2025, "001")`.
- **Resultado esperado:** item contem numero, valor e natureza de despesa esperados.

#### CT-097 - Cliente TCE retorna vazio em erro de empenhos

- **Metodo:** `GetEmpenhosByOrgaoAsync_ReturnsEmpty_WhenApiReturnsError`
- **Pre-condicoes:** handler retorna HTTP 404.
- **Dados:** orgao inexistente `999`.
- **Passos:** chamar metodo de empenhos por orgao.
- **Resultado esperado:** lista vazia, sem excecao.

#### CT-098 - Cliente TCE desserializa contratos

- **Metodo:** `GetContratosAsync_ReturnsContratos_WhenApiReturnsTceJson`
- **Pre-condicoes:** handler retorna HTTP 200.
- **Dados:** envelope JSON com um contrato.
- **Passos:** chamar `GetContratosAsync(2025)`.
- **Resultado esperado:** contrato retornado contem numero e valor esperados.

#### CT-099 - Cliente TCE retorna vazio em erro de contratos

- **Metodo:** `GetContratosAsync_ReturnsEmpty_WhenApiReturnsError`
- **Pre-condicoes:** handler retorna HTTP 503.
- **Dados:** ano 2025.
- **Passos:** chamar `GetContratosAsync`.
- **Resultado esperado:** lista vazia, sem excecao.

#### CT-100 - Cliente TCE retorna vazio em erro de orcamento

- **Metodo:** `GetOrcamentoAsync_ReturnsEmpty_WhenApiReturnsError`
- **Pre-condicoes:** handler retorna HTTP 502.
- **Dados:** ano 2025.
- **Passos:** chamar `GetOrcamentoAsync`.
- **Resultado esperado:** lista vazia, sem excecao.

### Middlewares - GlobalExceptionMiddleware

**Arquivo:** `tests/TransparenciaPE.UnitTests/Middlewares/GlobalExceptionMiddlewareTests.cs`

#### CT-101 - Middleware chama proximo delegate sem excecao

- **Metodo:** `InvokeAsync_CallsNext_WhenNoExceptionOccurs`
- **Pre-condicoes:** `RequestDelegate` apenas marca flag de chamada.
- **Dados:** `DefaultHttpContext` com corpo gravavel.
- **Passos:** executar `InvokeAsync`.
- **Resultado esperado:** proximo delegate e chamado e status permanece `200`.

#### CT-102 - Middleware mapeia excecoes para status HTTP

- **Metodo:** `InvokeAsync_ReturnsExpectedStatusCode_ForGivenException`
- **Pre-condicoes:** delegate configurado para lancar excecao.
- **Dados:** `ArgumentException`, `NotFoundException` e `InvalidOperationException`.
- **Passos:** executar middleware para cada excecao.
- **Resultado esperado:** status `400`, `404` e `500`, respectivamente.

#### CT-103 - Middleware retorna erro JSON estruturado

- **Metodo:** `InvokeAsync_ReturnsJsonError_WhenExceptionOccurs`
- **Pre-condicoes:** delegate lanca `ArgumentException`.
- **Dados:** contexto HTTP com `MemoryStream`.
- **Passos:** executar middleware, ler corpo e parsear JSON.
- **Resultado esperado:** resposta `application/json` com `statusCode`, `message` e `timestamp`.

### Conventions - UnitTests

**Arquivo:** `tests/TransparenciaPE.UnitTests/Conventions/TestNamingConventionTests.cs`

#### CT-104 - Convencao valida nomes dos testes unitarios

- **Metodo:** `TestMethods_FollowNamingConvention_WhenDeclaredInUnitAssembly`
- **Pre-condicoes:** assembly de testes unitarios carregado.
- **Dados:** metodos com `[Fact]` ou `[Theory]`.
- **Passos:** varrer metodos publicos declarados no assembly.
- **Resultado esperado:** todos seguem o padrao `Assunto_Comportamento_WhenCondicao`.

### IntegrationTests - API Endpoints

**Arquivo:** `tests/TransparenciaPE.IntegrationTests/ApiEndpointsTests.cs`

#### CT-105 - Endpoint de resumo retorna HTTP 200

- **Metodo:** `GetResumo_ReturnsOkStatus_WhenAnoIsProvided`
- **Pre-condicoes:** `CustomWebApplicationFactory` sobe API em ambiente `Testing`.
- **Dados:** `GET /api/v1/dashboard/resumo?ano=2025`.
- **Passos:** executar requisicao via `HttpClient`.
- **Resultado esperado:** status `200 OK`.

#### CT-106 - Endpoint de resumo retorna JSON

- **Metodo:** `GetResumo_ReturnsJsonContentType_WhenAnoIsProvided`
- **Pre-condicoes:** API de teste ativa.
- **Dados:** mesma rota de resumo.
- **Passos:** executar requisicao e ler header `Content-Type`.
- **Resultado esperado:** media type `application/json`.

#### CT-107 - Endpoint de resumo retorna payload de KPIs

- **Metodo:** `GetResumo_ReturnsDashboardResumoPayload_WhenAnoIsProvided`
- **Pre-condicoes:** `FakeDashboardService` retorna dados deterministas.
- **Dados:** ano 2025.
- **Passos:** desserializar resposta para `DashboardResumoDto`.
- **Resultado esperado:** `TotalEmpenhado=1000` e `PercentualExecutado=60`.

#### CT-108 - Endpoint de evolucao retorna HTTP 200

- **Metodo:** `GetEvolucao_ReturnsOkStatus_WhenCodigoOrgaoIsProvided`
- **Pre-condicoes:** API de teste ativa.
- **Dados:** `GET /api/v1/dashboard/evolucao?codigoOrgao=001&ano=2025`.
- **Passos:** executar requisicao.
- **Resultado esperado:** status `200 OK`.

#### CT-109 - Endpoint de evolucao retorna drill-down

- **Metodo:** `GetEvolucao_ReturnsDrillDownPayload_WhenCodigoOrgaoIsProvided`
- **Pre-condicoes:** `FakeDashboardService` retorna um item de drill-down.
- **Dados:** `codigoOrgao=001`.
- **Passos:** desserializar resposta para `DrillDownDto`.
- **Resultado esperado:** `CodigoOrgao="001"` e `Itens` contem um item.

#### CT-110 - Pesquisa global retorna 400 para termo vazio

- **Metodo:** `PesquisaGlobal_ReturnsBadRequestStatus_WhenTermoIsEmpty`
- **Pre-condicoes:** `FakePesquisaService` rejeita termo vazio.
- **Dados:** `GET /api/v1/pesquisa/global?termo=`.
- **Passos:** executar requisicao.
- **Resultado esperado:** status `400 BadRequest`.

#### CT-111 - Pesquisa global retorna payload de erro

- **Metodo:** `PesquisaGlobal_ReturnsErrorPayload_WhenTermoIsEmpty`
- **Pre-condicoes:** rota valida a falha do termo vazio.
- **Dados:** resposta de erro da API.
- **Passos:** parsear corpo como JSON e verificar `message` ou `errors`.
- **Resultado esperado:** payload possui mensagem ou erros de validacao.

#### CT-112 - Exportacao CSV retorna HTTP 200

- **Metodo:** `ExportarCsv_ReturnsOkStatus_WhenAnoIsProvided`
- **Pre-condicoes:** `FakePesquisaService` retorna bytes CSV.
- **Dados:** `GET /api/v1/exportar/csv?ano=2025`.
- **Passos:** executar requisicao.
- **Resultado esperado:** status `200 OK`.

#### CT-113 - Exportacao CSV retorna content-type correto

- **Metodo:** `ExportarCsv_ReturnsCsvContentType_WhenAnoIsProvided`
- **Pre-condicoes:** API de teste ativa.
- **Dados:** rota de exportacao CSV.
- **Passos:** ler `Content-Type`.
- **Resultado esperado:** media type `text/csv`.

#### CT-114 - Exportacao CSV retorna cabecalho

- **Metodo:** `ExportarCsv_ReturnsCsvHeader_WhenAnoIsProvided`
- **Pre-condicoes:** servico fake retorna CSV deterministico.
- **Dados:** ano 2025.
- **Passos:** ler conteudo como string.
- **Resultado esperado:** conteudo inicia com `NumeroEmpenho;Orgao;Valor`.

#### CT-115 - Exportacao CSV retorna nome de arquivo

- **Metodo:** `ExportarCsv_ReturnsCsvFileName_WhenAnoIsProvided`
- **Pre-condicoes:** resposta de arquivo gerada pelo controller.
- **Dados:** chamada com ano 2025.
- **Passos:** ler header `Content-Disposition`.
- **Resultado esperado:** nome de arquivo contem `.csv`.

### Conventions - IntegrationTests

**Arquivo:** `tests/TransparenciaPE.IntegrationTests/Conventions/TestNamingConventionTests.cs`

#### CT-116 - Convencao valida nomes dos testes de integracao

- **Metodo:** `TestMethods_FollowNamingConvention_WhenDeclaredInIntegrationAssembly`
- **Pre-condicoes:** assembly de integracao carregado.
- **Dados:** metodos com `[Fact]` ou `[Theory]`.
- **Passos:** varrer metodos publicos declarados no assembly.
- **Resultado esperado:** todos seguem o padrao `Assunto_Comportamento_WhenCondicao`.

---

## Observacoes de cobertura

- Os testes unitarios isolam dependencias com Moq sempre que ha repositorios, clientes HTTP, servicos ou loggers.
- Os testes de repositorio e contexto usam banco em memoria para validar comportamento de persistencia e mapeamento.
- Os testes de integracao usam `CustomWebApplicationFactory` com servicos fake para validar contratos HTTP da API sem depender de banco ou APIs externas.
- O objetivo dos CTs e manter um comportamento por metodo sempre que possivel; cenarios amplos foram quebrados em testes menores para facilitar diagnostico.
