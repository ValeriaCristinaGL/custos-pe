using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Interfaces;

namespace TransparenciaPE.API.Controllers;

/// <summary>
/// Dashboard and BI endpoints — high-performance reads from PostgreSQL.
/// </summary>
[ApiController]
[Route("api/v1/dashboard")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Returns consolidated KPIs (Total committed, settled, paid, budget vs executed).
    /// </summary>
    [HttpGet("resumo")]
    [ProducesResponseType(typeof(DashboardResumoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResumoDto>> GetResumo([FromQuery] int? ano)
    {
        var result = await _dashboardService.GetResumoAsync(ano);
        return Ok(result);
    }

    /// <summary>
    /// Returns comparative data across government agencies for a given year (RF001).
    /// </summary>
    [HttpGet("comparativo-orgaos")]
    [ProducesResponseType(typeof(ComparativoOrgaosDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ComparativoOrgaosDto>> GetComparativoOrgaos([FromQuery] int ano)
    {
        var result = await _dashboardService.GetComparativoOrgaosAsync(ano);
        return Ok(result);
    }

    /// <summary>
    /// Returns hierarchical drill-down data (Agency → Cost Item) (RF001).
    /// </summary>
    [HttpGet("drill-down")]
    [ProducesResponseType(typeof(DrillDownDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DrillDownDto>> GetDrillDown(
        [FromQuery] string codigoOrgao, [FromQuery] int? ano)
    {
        var result = await _dashboardService.GetDrillDownAsync(codigoOrgao, ano);
        return Ok(result);
    }
}
