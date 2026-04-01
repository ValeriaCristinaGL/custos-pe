using TransparenciaPE.Application.DTOs;

namespace TransparenciaPE.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResumoDto> GetResumoAsync(int? ano = null);
    Task<ComparativoOrgaosDto> GetComparativoOrgaosAsync(int ano);
    Task<DrillDownDto> GetDrillDownAsync(string codigoOrgao, int? ano = null);
}
