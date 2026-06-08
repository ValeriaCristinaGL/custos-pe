# Relatorio de testes e desempenho - 07/06/2026

## Objetivo

Registrar os resultados obtidos na revisao da suite de testes, cobertura e testes de desempenho com k6, para apresentacao academica.

## Ambiente usado

- Branch: `revisao-testes-k6`
- SDK .NET: `8.0.127`, fixado em `global.json`
- API local: `http://localhost:5034`
- Banco: PostgreSQL via `docker-compose-development.yml`
- Ferramentas: xUnit, FluentAssertions, Moq, Coverlet, ReportGenerator e k6

## O que foi revisado/implementado

- Padronizacao dos nomes dos testes no formato `Subject_ExpectedBehavior` ou `Subject_ExpectedBehavior_WhenCondition`.
- Inclusao de teste automatizado para validar a convencao de nomenclatura.
- Inclusao de testes de integracao com `WebApplicationFactory<Program>`.
- Inclusao de testes da camada Infrastructure usando EF Core InMemory.
- Ajuste para uso exclusivo do .NET 8.
- Revisao dos scripts `backend/run-tests.sh` e `backend/run-tests.bat`.
- Revisao dos testes k6: smoke, load, stress, soak e spike.
- Exclusao dos artefatos locais de cobertura e k6 do versionamento via `.gitignore`.

## Resultado dos testes automatizados

Execucao realizada com:

```bash
bash backend/run-tests.sh
```

Resultado:

| Indicador | Resultado |
| --------- | --------- |
| Testes unitarios | 110 aprovados |
| Testes de integracao | 4 aprovados |
| Total | 114 aprovados |
| Falhas | 0 |

## Cobertura

Relatorio local:

```text
backend/TestResults/CoverageReport/index.html
backend/TestResults/CoverageReport/Summary.txt
```

Resumo:

| Metrica | Resultado |
| ------- | --------- |
| Line coverage | 94.2% |
| Branch coverage | 83.3% |
| Method coverage | 86.1% |
| Linhas cobertas | 461 |
| Linhas nao cobertas | 28 |
| Linhas cobriveis | 489 |

Cobertura por assembly:

| Assembly | Cobertura |
| -------- | --------- |
| TransparenciaPE.API | 94.4% |
| TransparenciaPE.Application | 93.6% |
| TransparenciaPE.Domain | 91.2% |
| TransparenciaPE.Infrastructure | 95.3% |

Conclusao: a meta de 90% de cobertura de linhas foi atingida, chegando a 94.2%.

## k6 - Smoke test

Comando:

```bash
./k6/run-k6.sh smoke
```

Arquivo gerado:

```text
k6/results/smoke_20260607_224309.json
```

Resultado:

| Indicador | Resultado |
| --------- | --------- |
| Checks | 100% aprovados |
| Falhas HTTP | 0.00% |
| p95 | 10.28ms |

Interpretacao: o smoke test confirmou que a API estava de pe, respondendo corretamente e sem falhas HTTP.

## k6 - Load test

Comando:

```bash
./k6/run-k6.sh load
```

Arquivo gerado:

```text
k6/results/load_20260607_224953.json
```

Cenario configurado:

| Fluxo | Carga |
| ----- | ----- |
| Dashboard | ate 40 usuarios virtuais |
| Pesquisa | ate 8 usuarios virtuais |
| Exportacao CSV | ate 2 usuarios virtuais |
| Total maximo | 50 usuarios virtuais |
| Duracao | aproximadamente 10 minutos |

Resultado:

| Indicador | Resultado |
| --------- | --------- |
| Requisicoes HTTP | 7.970 |
| Checks executados | 24.006 |
| Checks com falha | 0 |
| Falhas HTTP | 0 |
| Iteracoes | 2.952 |
| Usuarios virtuais maximos | 50 |
| Latencia media | 1.49ms |
| p95 | 1.95ms |
| p99 | 2.31ms |
| Maior tempo observado | 50.56ms |

Thresholds do load:

| Meta | Resultado |
| ---- | --------- |
| Taxa de erro HTTP < 1% | Aprovado: 0% |
| p95 < 1.500ms | Aprovado: 1.95ms |
| p99 < 2.000ms | Aprovado: 2.31ms |

Interpretacao: com ate 50 usuarios virtuais simultaneos, a API respondeu sem falhas e com latencia muito abaixo dos limites definidos.

## Decisao sobre artefatos versionados

Os relatorios de cobertura em `backend/TestResults/` e os resultados k6 em `k6/results/` nao devem ser versionados.

Motivo:

- Sao gerados automaticamente a cada execucao.
- Mudam muitos arquivos a cada rodada.
- A pipeline pode publicar esses resultados como artefato.
- O repositorio deve manter scripts, testes e documentacao, nao os arquivos gerados.

## Conclusao geral

A suite atual combina testes unitarios, testes de integracao, testes de infraestrutura e testes de desempenho. A cobertura de linhas atingiu 94.2%, superando a meta de 90%. No teste de carga local, a API suportou o cenario de 50 usuarios virtuais sem falhas HTTP, com 100% dos checks aprovados e p99 de 2.31ms.

Para evolucao futura, o ideal e executar stress, spike e soak em ambiente de staging/homologacao, pois esses testes buscam limite, recuperacao e estabilidade prolongada do sistema.
