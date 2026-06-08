# Estrategia de testes

## Como executar

Na raiz do repositorio:

```bash
bash backend/run-tests.sh
```

No Windows:

```bat
backend\run-tests.bat
```

Os scripts executam os testes unitarios e de integracao da solution com .NET 8 e geram cobertura em:

```text
backend/TestResults/CoverageReport/index.html
backend/TestResults/CoverageReport/Summary.txt
```

`backend/TestResults/` e artefato de execucao local/pipeline e nao deve ser versionado.

## Testes unitarios

Projeto: `TransparenciaPE.UnitTests`

Cobrem regras isoladas de helpers, entidades, services, controllers, middlewares e clientes externos com mocks/fakes.

Convencao de nomes: `Subject_ExpectedBehavior` ou `Subject_ExpectedBehavior_WhenCondition`. A classe `TestNamingConventionTests` valida essa regra automaticamente.

Os testes de infraestrutura cobrem `AppDbContext`, configuracoes EF, repositorios, `UnitOfWork` e clientes fake com provider EF InMemory. O `DapperDashboardQueryService` depende de SQL PostgreSQL/Npgsql e deve ser testado com banco efemero ou apos refatoracao para injecao de conexao.

## Testes de integracao

Projeto: `TransparenciaPE.IntegrationTests`

Cobrem a API por HTTP com `WebApplicationFactory<Program>`, validando roteamento, serializacao, content types e tratamento de erro sem depender de banco real. O ambiente `Testing` desabilita worker de sincronizacao e migrations de startup.

## Testes de desempenho

Pasta: `../../k6`

Cobrem smoke, load, stress, soak e spike tests dos endpoints publicos. Resultados gerados em `k6/results/` sao artefatos locais e ficam fora do versionamento.

Execucao recomendada:

```bash
docker compose -f docker-compose-development.yml up -d postgres backend
./k6/run-k6.sh smoke
./k6/run-k6.sh load
```

O smoke valida disponibilidade basica. O load valida a API sob carga normal, comparando latencia e taxa de erro com os thresholds de `k6/config/thresholds.js`.

## Resultado de referencia

Ultima execucao validada em 07/06/2026:

- 114 testes automatizados aprovados.
- 94.2% de line coverage.
- 83.3% de branch coverage.
- 86.1% de method coverage.
- Load test k6 aprovado com 7.970 requisicoes, 0 falhas HTTP e p99 de 2.31ms.

## Proximos tipos recomendados

- Testes de contrato para garantir formato de payloads consumidos pelo frontend.
- Testes end-to-end do frontend para fluxos criticos de dashboard, pesquisa e exportacao CSV.
- Testes de repositorio com banco efemero quando houver pipeline preparado para PostgreSQL de teste.
