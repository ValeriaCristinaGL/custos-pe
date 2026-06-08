![Coverage](https://img.shields.io/badge/Coverage-94.2%25-brightgreen)

<!-- COVERAGE_BADGE -->
<div align="center">

# 📊 TransparênciaPE

**Plataforma de inteligência fiscal para o Estado de Pernambuco.**  
Transformamos dados financeiros complexos em informações claras e acessíveis para todo cidadão.

[![Frontend CI/CD](https://github.com/ValeriaCristinaGL/custos-pe/actions/workflows/frontend-ci-cd.yml/badge.svg)](https://github.com/ValeriaCristinaGL/custos-pe/actions/workflows/frontend-ci-cd.yml)
[![Backend CI/CD](https://github.com/ValeriaCristinaGL/custos-pe/actions/workflows/backend-ci-cd.yml/badge.svg)](https://github.com/ValeriaCristinaGL/custos-pe/actions/workflows/backend-ci-cd.yml)

</div>

---

## ✨ Visão Geral

O **TransparênciaPE** é um sistema ETL + Dashboard que extrai, higieniza e consolida os dados das APIs governamentais do TCE-PE, entregando painéis visuais interativos acessíveis a cidadãos, gestores públicos, jornalistas e auditores.

| Funcionalidade              | Descrição                                                          |
| --------------------------- | ------------------------------------------------------------------ |
| **Painel Geral**            | KPIs de despesa total, receita e investimentos com evolução mensal |
| **Comparação entre Órgãos** | Análise comparativa de desempenho entre secretarias                |
| **Análise Temporal**        | Sazonalidade, tendências e projeções históricas                    |
| **Pesquisa Global**         | Busca por fornecedor, CNPJ ou número de contrato/empenho           |
| **Exportação CSV**          | Download dos dados filtrados em formato aberto                     |

---

## 🏗️ Arquitetura

```
custos-pe/
├── frontend/          # React 19 + Vite + TypeScript + Tailwind CSS
├── backend/           # .NET 8 — Clean Architecture
│   ├── src/
│   │   ├── TransparenciaPE.API            # Controllers, Middlewares, Background Workers
│   │   ├── TransparenciaPE.Application    # Use Cases, Services, DTOs
│   │   ├── TransparenciaPE.Domain         # Entidades, Regras de Negócio
│   │   └── TransparenciaPE.Infrastructure # Repositórios, Dapper, EF Core, TCE-PE Client
│   └── tests/
│       ├── TransparenciaPE.UnitTests      # xUnit + Moq + FluentAssertions
│       └── TransparenciaPE.IntegrationTests # WebApplicationFactory + API HTTP
├── k6/               # Smoke, load, stress, soak e spike tests
├── docker-compose-development.yml
└── docker-compose-prod.yml
```

**Stack técnica:**

| Camada         | Tecnologia                                           |
| -------------- | ---------------------------------------------------- |
| Frontend       | React 19, TypeScript, Vite, Tailwind CSS 4, Recharts |
| Backend        | .NET 8, C#, ASP.NET Core                             |
| ORM / Queries  | Entity Framework Core + Dapper (CQRS-Lite)           |
| Banco de dados | PostgreSQL 14                                        |
| Testes         | xUnit, Moq, FluentAssertions, Coverlet, k6           |
| Infraestrutura | Docker, GitHub Actions CI/CD                         |
| Logs           | Serilog                                              |

---

## 🚀 Rodando com Docker (recomendado)

> Pré-requisito: [Docker](https://www.docker.com/) instalado.

```bash
# Clone o repositório
git clone https://github.com/ValeriaCristinaGL/custos-pe.git
cd custos-pe

# Suba todo o ambiente (PostgreSQL + Backend + Frontend)
docker compose -f docker-compose-development.yml up --build
```

| Serviço     | URL                           |
| ----------- | ----------------------------- |
| Frontend    | http://localhost:5173         |
| Backend API | http://localhost:5034         |
| Swagger UI  | http://localhost:5034/swagger |

---

## 🛠️ Rodando localmente (sem Docker)

### Backend

> Pré-requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) e PostgreSQL.

```bash
cd backend

# Configure a connection string em:
# src/TransparenciaPE.API/appsettings.Development.json

# Aplique as migrações
dotnet ef database update \
  --project src/TransparenciaPE.Infrastructure \
  --startup-project src/TransparenciaPE.API

# Inicie a API
dotnet run --project src/TransparenciaPE.API
```

### Frontend

> Pré-requisito: Node.js 20+

```bash
cd frontend

# Instale as dependências
npm install

# Crie o arquivo de variáveis de ambiente
echo "VITE_API_URL=http://localhost:5034" > .env.local

# Inicie o servidor de desenvolvimento
npm run dev
```

---

## 🧪 Testes

### Backend e cobertura

```bash
# Executa toda a suíte e gera relatório de cobertura
bash backend/run-tests.sh        # Linux/macOS
backend\run-tests.bat            # Windows
```

O relatório HTML de cobertura é gerado em `backend/TestResults/CoverageReport/index.html`.
A pasta `backend/TestResults/` é artefato local/pipeline e não deve ser versionada.

**Cobertura atual:** 94.2% de Line Coverage · 83.3% de Branch Coverage · 86.1% de Method Coverage.

### Testes de desempenho com k6

```bash
# Suba a API local com banco de desenvolvimento
docker compose -f docker-compose-development.yml up -d postgres backend

# Smoke test: valida se a API está respondendo
./k6/run-k6.sh smoke

# Load test: simula carga normal, com até 50 usuários virtuais
./k6/run-k6.sh load
```

Os resultados JSON do k6 são gerados em `k6/results/`, que também é artefato local e fica fora do versionamento. O guia completo fica em `k6/README.md`.

---

## 🌐 Deploy (Vercel)

O frontend está configurado para deploy automático na Vercel via `vercel.json` na raiz do repositório.

**Variável de ambiente obrigatória no painel da Vercel:**

| Variável       | Valor                      |
| -------------- | -------------------------- |
| `VITE_API_URL` | URL do backend em produção |

---

## 📐 Qualidade e Padrões

- **Clean Architecture** — Domain · Application · Infrastructure · API completamente desacoplados
- **SOLID** — Inversão de dependências via Interfaces nativas do .NET DI
- **CQRS-Lite** — Escritas via EF Core, leituras otimizadas via Dapper
- **Rate Limiting** — Proteção contra abuso de API e ataques DDoS
- **GlobalExceptionMiddleware** — Nenhum stack trace exposto ao usuário (RFC 7807)
- **CI/CD bloqueante** — PRs só chegam à `main` após build limpa + testes passando
- **Commits semânticos** — Rastreabilidade total do histórico

---

## 👥 Equipe

| Nome                             | Papel             |
| -------------------------------- | ----------------- |
| Valéria Cristina Galindo de Lima | Frontend & DevOps |
| Nathan Maciel Vieira da Rocha    | Backend           |
| Ítalo Ruan Ribeiro Soares        | Backend           |
| Kauanne Cavalcante de Andrade    | Frontend          |
| Pedro Victor da Silva Pereira    | Backend           |
| Tomás Abdias dos Anjos Lima       | QA                |


---

<div align="center">

**IFPE Campus Belo Jardim · Bacharelado em Engenharia de Software**  
Dados públicos, acesso democrático.

</div>
