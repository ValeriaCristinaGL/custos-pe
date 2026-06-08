# Testes de Carga k6 — Transparência PE

Suíte de testes de desempenho para a API do portal de custos do estado de Pernambuco.

## Estrutura

```
k6/
├── config/
│   └── thresholds.js       # Thresholds (SLAs) por tipo de teste
├── tests/
│   ├── smoke.js             # Smoke test  — valida que a API está em pé
│   ├── load.js              # Load test   — carga normal de produção
│   ├── stress.js            # Stress test — ponto de ruptura do sistema
│   ├── soak.js              # Soak test   — detecta degradação gradual
│   └── spike.js             # Spike test  — picos repentinos de tráfego
├── utils/
│   └── helpers.js           # Métricas, checks e dados de exemplo
├── results/                 # Resultados JSON gerados automaticamente
└── run-k6.sh                # Script auxiliar de execução
```

## Endpoints cobertos

| Endpoint                        | Método | Descrição                                            |
| ------------------------------- | ------ | ---------------------------------------------------- |
| `/api/v1/dashboard/resumo`      | GET    | KPIs consolidados (total empenhado, liquidado, pago) |
| `/api/v1/dashboard/comparativo` | GET    | Comparativo entre órgãos por ano                     |
| `/api/v1/dashboard/evolucao`    | GET    | Drill-down por órgão (evolução mensal)               |
| `/api/v1/pesquisa/global`       | GET    | Busca por CNPJ, fornecedor ou nº de contrato         |
| `/api/v1/exportar/csv`          | GET    | Exportação dos dados filtrados em CSV                |

## Pré-requisitos

Instale o k6:

```bash
# Linux (Debian/Ubuntu)
sudo gpg -k
sudo gpg --no-default-keyring \
  --keyring /usr/share/keyrings/k6-archive-keyring.gpg \
  --keyserver hkp://keyserver.ubuntu.com:80 \
  --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" \
  | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update && sudo apt-get install k6

# macOS
brew install k6

# Windows (Chocolatey)
choco install k6

# Docker (sem instalação)
docker run --rm -i --network host grafana/k6 run - < k6/tests/smoke.js
```

## Executar os testes

### Preparar a API local

O script usa `http://localhost:5034` como URL padrao. Para subir a API com PostgreSQL pelo Docker Compose de desenvolvimento:

```bash
docker compose -f docker-compose-development.yml up -d postgres backend

# Validacao rapida da API
curl -I http://localhost:5034/api/v1/dashboard/resumo
```

Se for executar contra outro ambiente, informe `BASE_URL`:

```bash
BASE_URL=https://staging.transparenciape.pe.gov.br ./k6/run-k6.sh smoke
```

### Via script auxiliar (recomendado)

```bash
chmod +x k6/run-k6.sh

# Smoke test (rápido — ~1 minuto)
./k6/run-k6.sh smoke

# Load test (~10 minutos)
./k6/run-k6.sh load

# Stress test (~18 minutos) ⚠️ Apenas em staging
./k6/run-k6.sh stress

# Soak test (padrão: 1 hora)
./k6/run-k6.sh soak

# Soak test reduzido para CI (30 minutos)
SOAK_DURATION=30m ./k6/run-k6.sh soak

# Spike test (~12 minutos) ⚠️ Apenas em staging
./k6/run-k6.sh spike

# Smoke + Load em sequência (indicado para CI/CD)
./k6/run-k6.sh all

# Execução não interativa em CI
K6_NON_INTERACTIVE=true ./k6/run-k6.sh all

# Stress/spike em CI exigem confirmação explícita
K6_NON_INTERACTIVE=true K6_CONFIRM_DESTRUCTIVE=true ./k6/run-k6.sh stress
```

### Diretamente com k6

```bash
# URL padrão: http://localhost:5034
k6 run k6/tests/smoke.js

# Apontando para outro ambiente
k6 run -e BASE_URL=https://staging.transparenciape.pe.gov.br k6/tests/load.js

# Salvando resultado em JSON para análise posterior
k6 run --out json=k6/results/load_result.json k6/tests/load.js
```

## Tipos de teste e quando usar

| Teste      | VUs máx. | Duração | Quando usar                                  |
| ---------- | -------- | ------- | -------------------------------------------- |
| **Smoke**  | 1        | 1 min   | Antes de qualquer deploy; verificação rápida |
| **Load**   | 50       | 10 min  | Validação de SLA em carga normal             |
| **Stress** | 200      | 18 min  | Encontrar limites antes de escalar           |
| **Soak**   | 20       | 1 hora  | Detectar memory leaks em staging             |
| **Spike**  | 500      | 12 min  | Simular exposição em mídia/relatórios        |

## SLAs / Thresholds

| Métrica          | Load test | Stress test |
| ---------------- | --------- | ----------- |
| Taxa de erro     | < 1%      | < 5%        |
| p50 latência     | < 500ms   | —           |
| p95 latência     | < 1,5s    | < 3s        |
| p99 latência     | < 2s      | < 5s        |
| Exportar CSV p95 | < 5s      | —           |

## Integração com CI/CD

Adicione ao seu pipeline (ex.: GitHub Actions):

```yaml
- name: Instalar k6
  run: |
    sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 \
      --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
    echo "deb https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
    sudo apt-get update && sudo apt-get install k6

- name: Executar smoke + load tests
  env:
    BASE_URL: ${{ secrets.STAGING_URL }}
  run: ./k6/run-k6.sh all
```

## Analisando resultados

Os resultados JSON ficam em `k6/results/`. Essa pasta é artefato local de execução e deve permanecer fora do versionamento.

Na saida do terminal, os principais campos sao:

| Campo | Como interpretar |
| ----- | ---------------- |
| `checks_succeeded` | Percentual de validacoes funcionais que passaram |
| `http_req_failed` | Percentual de requisicoes HTTP com falha |
| `http_req_duration p(95)` | 95% das requisicoes responderam abaixo desse tempo |
| `http_req_duration p(99)` | 99% das requisicoes responderam abaixo desse tempo |
| `vus` / `vus_max` | Usuarios virtuais ativos e limite maximo configurado |
| `iterations` | Quantidade de fluxos completos executados |
| `thresholds` | Metas de SLA; se aparecerem como aprovadas, o teste passou |

Exemplo do load test validado em 07/06/2026:

| Metrica | Resultado |
| ------- | --------- |
| VUs maximos | 50 |
| Requisicoes HTTP | 7.970 |
| Checks | 24.006 de 24.006 aprovados |
| Falhas HTTP | 0 |
| Latencia media | 1.49ms |
| p95 | 1.95ms |
| p99 | 2.31ms |

Conclusao: no cenario local de carga normal, a API ficou estavel, sem erros e bem abaixo dos SLAs definidos.

Para visualizá-los com Grafana:

```bash
# Subir k6 + InfluxDB + Grafana com Docker Compose
k6 run --out influxdb=http://localhost:8086/k6 k6/tests/load.js
```

Consulte a [documentação oficial do k6](https://k6.io/docs/results-visualization/) para mais opções de visualização.
