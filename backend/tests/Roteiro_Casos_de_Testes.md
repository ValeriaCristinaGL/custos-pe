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

## Observacoes de cobertura

- Os testes unitarios isolam dependencias com Moq sempre que ha repositorios, clientes HTTP, servicos ou loggers.
- Os testes de repositorio e contexto usam banco em memoria para validar comportamento de persistencia e mapeamento.
- Os testes de integracao usam `CustomWebApplicationFactory` com servicos fake para validar contratos HTTP da API sem depender de banco ou APIs externas.
- O objetivo dos CTs e manter um comportamento por metodo sempre que possivel; cenarios amplos foram quebrados em testes menores para facilitar diagnostico.
