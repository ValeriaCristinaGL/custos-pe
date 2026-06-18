using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Interfaces;

namespace TransparenciaPE.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDashboardService>();
            services.RemoveAll<IPesquisaService>();

            services.AddSingleton<IDashboardService, FakeDashboardService>();
            services.AddSingleton<IPesquisaService, FakePesquisaService>();
        });
    }

    private sealed class FakeDashboardService : IDashboardService
    {
        public Task<DashboardResumoDto> GetResumoAsync(int? ano = null)
        {
            return Task.FromResult(new DashboardResumoDto
            {
                TotalEmpenhado = 1000m,
                TotalLiquidado = 800m,
                TotalPago = 600m,
                PercentualExecutado = 60m,
                TotalEmpenhos = 10,
                TotalContratos = 2
            });
        }

        public Task<ComparativoOrgaosDto> GetComparativoOrgaosAsync(int ano)
        {
            return Task.FromResult(new ComparativoOrgaosDto
            {
                Ano = ano,
                Orgaos =
                [
                    new OrgaoComparativoItem
                    {
                        CodigoOrgao = "001",
                        NomeOrgao = "Secretaria de Educacao",
                        SiglaOrgao = "SEDUC",
                        TotalEmpenhado = 1000m,
                        TotalLiquidado = 800m,
                        TotalPago = 600m
                    }
                ]
            });
        }

        public Task<DrillDownDto> GetDrillDownAsync(string codigoOrgao, int? ano = null)
        {
            return Task.FromResult(new DrillDownDto
            {
                CodigoOrgao = codigoOrgao,
                NomeOrgao = "Secretaria de Educacao",
                Itens =
                [
                    new DrillDownItem
                    {
                        ClassificacaoMcasp = "Custeio",
                        Descricao = "Material de consumo",
                        TotalEmpenhado = 100m,
                        QuantidadeEmpenhos = 3
                    }
                ]
            });
        }
    }

    private sealed class FakePesquisaService : IPesquisaService
    {
        public Task<PesquisaResultDto> PesquisaGlobalAsync(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                throw new ArgumentException("O termo de busca e obrigatorio.");
            }

            return Task.FromResult(new PesquisaResultDto
            {
                TermoBuscado = termo,
                TotalResultados = 1,
                Resultados =
                [
                    new PesquisaItem
                    {
                        Tipo = "Empenho",
                        Numero = "EMP-001",
                        OrgaoNome = "Secretaria de Educacao",
                        Fornecedor = "Empresa A",
                        Cnpj = "11222333000181",
                        Valor = 100m,
                        Data = new DateTime(2025, 1, 1),
                        Descricao = "Material de consumo"
                    }
                ]
            });
        }

        public Task<byte[]> ExportarCsvAsync(string? termo = null, int? ano = null)
        {
            const string csv = "NumeroEmpenho;Orgao;Valor\nEMP-001;Secretaria de Educacao;100";
            return Task.FromResult(Encoding.UTF8.GetBytes(csv));
        }
    }
}
