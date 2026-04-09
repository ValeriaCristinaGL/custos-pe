using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Interfaces;
using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardQueryService _queryService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IDashboardQueryService queryService, ILogger<DashboardService> logger)
    {
        _queryService = queryService;
        _logger = logger;
    }

    public async Task<DashboardResumoDto> GetResumoAsync(int? ano = null)
    {
        _logger.LogInformation("Fetching dashboard summary for year {Ano}", ano?.ToString() ?? "all");

        var result = await _queryService.GetResumoAsync(ano);

        var percentualExecutado = result.TotalEmpenhado > 0
            ? Math.Round(result.TotalPago / result.TotalEmpenhado * 100, 2)
            : 0m;

        return new DashboardResumoDto
        {
            TotalEmpenhado = result.TotalEmpenhado,
            TotalLiquidado = result.TotalLiquidado,
            TotalPago = result.TotalPago,
            PercentualExecutado = percentualExecutado,
            TotalEmpenhos = result.TotalEmpenhos,
            TotalContratos = result.TotalContratos
        };
    }

    public async Task<ComparativoOrgaosDto> GetComparativoOrgaosAsync(int ano)
    {
        _logger.LogInformation("Fetching agency comparison for year {Ano}", ano);

        var results = await _queryService.GetComparativoOrgaosAsync(ano);

        return new ComparativoOrgaosDto
        {
            Ano = ano,
            Orgaos = results.Select(r => new OrgaoComparativoItem
            {
                CodigoOrgao = r.CodigoOrgao,
                NomeOrgao = r.NomeOrgao,
                SiglaOrgao = r.SiglaOrgao,
                TotalEmpenhado = r.TotalEmpenhado,
                TotalLiquidado = r.TotalLiquidado,
                TotalPago = r.TotalPago
            })
        };
    }

    public async Task<DrillDownDto> GetDrillDownAsync(string codigoOrgao, int? ano = null)
    {
        _logger.LogInformation("Fetching drill-down for agency {CodigoOrgao}", codigoOrgao);

        var results = await _queryService.GetDrillDownAsync(codigoOrgao, ano);

        return new DrillDownDto
        {
            CodigoOrgao = codigoOrgao,
            Itens = results.Select(r => new DrillDownItem
            {
                ClassificacaoMcasp = r.ClassificacaoMcasp,
                Descricao = r.Descricao,
                TotalEmpenhado = r.TotalEmpenhado,
                QuantidadeEmpenhos = r.QuantidadeEmpenhos
            })
        };
    }
}
