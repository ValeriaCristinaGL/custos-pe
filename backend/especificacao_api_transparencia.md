Especificação Técnica: API de Transparência de Custos (Pernambuco)

Este documento detalha as funcionalidades, arquitetura e requisitos para o desenvolvimento de uma API em .NET 8. O objetivo é consolidar e apresentar os custos do Estado de Pernambuco em conformidade com o MCASP e a LAI.

Nota para IA de Desenvolvimento: Este documento serve como o "System Prompt" ou "Technical Specification" para a geração do código. A implementação deve seguir os padrões de Clean Architecture e utilizar PostgreSQL como persistência de dados.

1. Origem dos Dados e Integração (Data Sourcing)

A principal responsabilidade desta API é atuar como um Orquestrador e Processador de Dados. Os dados não são gerados internamente, mas consumidos de fontes governamentais externas.

Fonte Externa: API do Portal da Transparência do Estado de Pernambuco (ou similar da SEFAZ-PE).

Fase de Descoberta: O projeto deve iniciar com um módulo de Data Discovery para mapear os endpoints da API oficial (ex: empenho, liquidação, pagamento e contratos).

Camada de Integração: Utilizar Refit ou IHttpClientFactory para o consumo.

Resiliência: Implementar políticas de Retry e Circuit Breaker (Polly).

2. Arquitetura e Tech Stack

Framework: .NET 8 (Web API).

Padrão de Design: Aggregator Pattern (Consome externo, processa e persiste localmente para servir o frontend).

Banco de Dados (Persistência e BI): PostgreSQL. Toda a inteligência de dados, históricos e agregações deve residir no PostgreSQL para garantir autonomia e performance.

ORM: * Entity Framework Core: Utilizado para gestão de esquema (Migrations), escrita de dados e operações administrativas.

Dapper: Utilizado para consultas de leitura de alta performance nos endpoints de Dashboard e BI.

Documentação: Swagger com anotações XML.

3. Endpoints da API

3.1. Dashboards e BI (Consumo do PostgreSQL)

GET /api/v1/dashboard/resumo: KPIs consolidados (Total gasto, Orçamento vs Executado).

GET /api/v1/dashboard/comparativo-orgaos: Cruzamento de dados entre secretarias (RF001).

GET /api/v1/dashboard/drill-down: Detalhamento hierárquico (Secretaria -> Órgão -> Item de Custo).

3.2. Pesquisa e Transparência Ativa

GET /api/v1/pesquisa/global: Busca por CNPJ, Nome de Fornecedor ou Contrato (RF003).

GET /api/v1/exportar/csv: Exportação dos filtros atuais em formato aberto (RF002).

4. Requisitos de Processamento (Sincronização PostgreSQL)

Como a API externa pode ser instável ou lenta, a solução deve implementar um fluxo de persistência local:

Background Worker: Um Worker Service em .NET que sincroniza periodicamente os dados da API externa de Pernambuco para o banco PostgreSQL.

Mapeamento MCASP: Durante o processo de salvamento no PostgreSQL, os dados brutos devem ser convertidos para a taxonomia do MCASP.

Estratégia de Upsert: A IA deve implementar lógica de Upsert (Insert or Update) baseada em chaves únicas de contratos/empenhos para evitar duplicidade no PostgreSQL.

Sanitização: Limpeza de strings e normalização de CNPJs antes da persistência.

5. Segurança e Qualidade (RQ)

Performance (RQ001): As consultas aos dashboards devem ser feitas diretamente no PostgreSQL (usando índices adequados e Dapper) para garantir resposta em < 3 segundos.

Segurança (RQ003): Rate Limiting e sanitização de queries para prevenir SQL Injection.

Disponibilidade: A aplicação deve continuar funcionando com os dados salvos no PostgreSQL mesmo se a API externa de Pernambuco estiver temporariamente fora do ar.

6. Diretrizes para Programação via IA

Ao gerar o código para esta solução, a IA deve:

Infraestrutura PostgreSQL: Configurar o DbContext para utilizar o driver Npgsql.

Mock de Integração: Criar a interface IPEDataClient para abstrair a API externa.

Migrations: Gerar as classes de entidade de forma que o Entity Framework possa criar as tabelas no PostgreSQL com os tipos de dados corretos (ex: numeric para valores financeiros).

Performance: Sugerir a criação de índices nas colunas de busca (CNPJ, Nome do Órgão, Data) no PostgreSQL.