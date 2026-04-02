# Transparência PE Backend - Guia de Configuração e Testes

Este projeto compõe a camada backend (APIs do Dashboard e WebAPI Aggregator). Abaixo detalhamos como instalar as dependências e colocar o serviço no ar.

## Conectando ao Banco de Dados (Variáveis de Ambiente)
A WebAPI utiliza **PostgreSQL** para o armazenamento persistente.
1. Confirme as credenciais da sua instância de BD no arquivo `src/TransparenciaPE.API/appsettings.Development.json` (se não existir, baseie-se em `appsettings.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TransparenciaPE_DB;Username=postgres;Password=sua_senha"
  }
}
```
2. Aplique as migrações usando o Entity Framework Core pela raiz do projeto:
```bash
dotnet ef database update --project src\TransparenciaPE.Infrastructure --startup-project src\TransparenciaPE.API
```

## Como Iniciar a Aplicação (Rodar o Projeto)

Após mapear o banco de dados e instalar as ferramentas necessárias do .NET 8 (SDK), você pode utilizar a interface de linha de comando ou a IDE:
```bash
# Na raiz da pasta do subprojeto (backend)
dotnet run --project src\TransparenciaPE.API
```
> O sistema rodará com Swagger habilitado e acessível pelas portas expostas (ex: `https://localhost:7165/swagger` ou `http://localhost:5034/swagger`). Além disso, o *Worker* de sincronização iniciará o laço de ingestão automaticamente em Background.

## Como Rodar os Testes (TDD/Unitários)

A suite de testes cobre mapeamento financeiro MCASP, manipulações do cliente HttpClient externo do TCE-PE e integrações das regras da camada de Serviço. 

Utilize o runner do xUnit via terminal para executar e tabular as validações:
```bash
dotnet test tests\TransparenciaPE.UnitTests
```
*Garantimos 100% de passagem de teste na construção atual! Todos os Mocks foram implementados garantindo segurança contra reações falsas do provedor real.*
