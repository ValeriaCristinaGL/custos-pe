using CustosPE.API.Domain.DTOs;

namespace CustosPE.API.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardDTO> GetDashboardAsync(int ano);
    Task<IEnumerable<DespesaPorCategoriaDTO>> GetDespesasPorCategoriaAsync(int ano);
    Task<IEnumerable<ReceitaPorCategoriaDTO>> GetReceitasPorCategoriaAsync(int ano);
    Task<IEnumerable<DespesaPorOrgaoDTO>> GetTopOrgaosPorDespesaAsync(int ano, int top = 10);
    Task<IEnumerable<EvolucaoMensalDTO>> GetEvolucaoMensalAsync(int ano);
}
