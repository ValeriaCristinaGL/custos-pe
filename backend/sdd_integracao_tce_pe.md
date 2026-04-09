# SDD - Plano de Desenvolvimento: Integração de Dados Reais (TCE-PE)

Este documento detalha o plano técnico para integrar a API de Dados Abertos do TCE-PE com a aplicação **Transparência PE**, visando atender aos requisitos visuais do protótipo (dashboards de Resumo, Comparação e Análise Temporal).

## 1. Mapeamento de Requisitos Visuais vs. Fonte de Dados

| Componente UI | Métrica | Fonte de Dados (Método TCE-PE) | Lógica de Cálculo / Filtro |
| :--- | :--- | :--- | :--- |
| **Painel Geral** | Despesa Total | `DespesasEstaduais` | `SUM(VALOR_EMPENHADO)` |
| | Receita Total | `ReceitasEstaduais` | `SUM(VALOR_RECEITA)` |
| | Investimentos | `DespesasEstaduais` | `SUM(VALOR)` onde Natureza inicia com '4.4' |
| | Evolução Mensal | `DespesasEstaduais` | Agrupamento por mês da `DATA_EMPENHO` |
| | Distribuição | `DespesasEstaduais` | Agrupamento por `NATUREZA_DESPESA` (Mcasp) |
| | Maiores Órgãos | `DespesasEstaduais` | Agrupamento por `NOME_UNIDADE_GESTORA` |
| **Comparação** | Perfil (Radar) | `DespesasEstaduais` | Filtros por Classe de Despesa (Pessoal, Invest., Custeio) |
| | Orçamento | `OrcamentoDespesa` | `SUM(VALOR_DOTACAO)` |
| | Executado | `EmpenhoLiquidacao` | `SUM(VALOR_LIQUIDADO)` |
| | Servidores | `ListaServidores` | `COUNT(*)` por Órgão |
| **Análise Temporal**| Crescimento | Vários Métodos | Comparativo histórico (Ano N vs Ano N-1) |

## 2. Padrão de Comunicação com a API TCE-PE

A URL base será `https://sistemas.tce.pe.gov.br/DadosAbertos/`.
O formato de retorno será sempre `json`.
A estrutura de resposta padrão é:

```json
{
  "resposta": {
    "status": "OK",
    "entidade": "NomeDoMetodo",
    "tamanhoResultado": "X",
    "limiteResultado": "10000",
    "conteudo": [ ... ]
  }
}
```

> [!IMPORTANT]
> A API do TCE-PE possui um limite rígido de **10.000 registros**. Para anos com volume superior (especialmente Despesas), implementaremos sincronização fatiada por Unidade Gestora ou Mês.

## 3. Alterações na Arquitetura do Domínio

### 3.1. Novas Entidades e Propriedades
Para suportar o radar chart e os novos cards, expandiremos o modelo:

*   **`OrgaoGoverno`**: Adicionar `TotalServidores` (int), `OrcamentoAtual` (decimal).
*   **`Receita` [NEW]**: Entidade para armazenar dados de `ReceitasEstaduais`.
*   **`Orcamento` [NEW]**: Histórico de dotação inicial e atual por órgão.
*   **`Empenho`**: Garantir que campos como `Funcao`, `Subfuncao` e `NaturezaDespesa` estejam bem mapeados para o dashboard.

### 3.2. Expansão da Interface `IPEDataClient`
```csharp
public interface IPEDataClient
{
    Task<IEnumerable<ExternalEmpenhoData>> GetEmpenhosAsync(int ano);
    Task<IEnumerable<ExternalContratoData>> GetContratosAsync(int ano);
    Task<IEnumerable<ExternalReceitaData>> GetReceitasAsync(int ano);
    Task<IEnumerable<ExternalOrcamentoData>> GetOrcamentoAsync(int ano);
    Task<int> GetTotalServidoresAsync(string codigoOrgao);
}
```

## 4. Estratégia de Sincronização (`DataSyncService`)

Implementaremos um worker de sincronização que segue esta ordem:
1.  **Sincronizar Órgãos**: Atualizar lista de UGs (`UnidadesJurisdicionadasEstaduais`).
2.  **Sincronizar Orçamento**: Buscar dotação de cada órgão para o ano atual.
3.  **Sincronizar Receitas**: Volume mensal consolidado.
4.  **Sincronizar Despesas**: Devido ao volume, faremos loops por Órgão para não estourar o limite de 10k.
5.  **Cálculo de Métricas**: Processar o `ClassificacaoMcasp` (Pessoal, Custeio, Investimento) durante o insert.

## 5. Melhorias na API de Dashboard

Os endpoints atuais serão adequados para retornar exatamente o que o protótipo visual pede:

- **`GET /api/v1/dashboard/resumo`**: Retornará as métricas de Despesa, Receita, Investimentos e crescimento vs ano anterior.
- **`GET /api/v1/dashboard/comparativo`**: Retornará dados para o Radar Chart (Perfil de Gastos) e a tabela de Orçamento vs Executado.
- **`GET /api/v1/dashboard/evolucao`**: Dados formatados para o gráfico de linhas (Evolução mensal).

## 6. Plano de Implementação (Fases)

### Fase 1: Infraestrutura de Client Real
- Craiar `TcePEDataClient` e mapear DTOs JSON.
- Implementar resiliência com Polly (Timeout de 60s).

### Fase 2: Expansão do Modelo de Dados
- Criar migrações para as novas entidades de Receita e Orçamento.
- Atualizar Repositórios Dapper.

### Fase 3: Sincronização e Processamento
- Ajustar `DataSyncService` para suportar fatiamento de dados (UG/Mês).
- Implementar `McaspMapper` robusto para categorizar despesas automaticamente.

### Fase 4: Dashboards e Visualização
- Adequar os DTOs do `DashboardService` ao layout do Figma/Imagem.
- Implementar agregação SQL eficiente para grandes volumes de dados.

---
**Autor:** Antigravity (IA)
**Data:** 02/04/2026
