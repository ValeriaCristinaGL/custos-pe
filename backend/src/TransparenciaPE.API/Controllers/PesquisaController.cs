using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Interfaces;

namespace TransparenciaPE.API.Controllers;

/// <summary>
/// Search and active transparency endpoints (RF002, RF003).
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class PesquisaController : ControllerBase
{
    private readonly IPesquisaService _pesquisaService;
    private readonly ILogger<PesquisaController> _logger;

    public PesquisaController(IPesquisaService pesquisaService, ILogger<PesquisaController> logger)
    {
        _pesquisaService = pesquisaService;
        _logger = logger;
    }

    /// <summary>
    /// Global search by CNPJ, supplier name or contract number (RF003).
    /// </summary>
    [HttpGet("pesquisa/global")]
    [ProducesResponseType(typeof(PesquisaResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PesquisaResultDto>> PesquisaGlobal([FromQuery] string termo)
    {
        try
        {
            var result = await _pesquisaService.PesquisaGlobalAsync(termo);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Exports current filters as open-format CSV (RF002).
    /// </summary>
    [HttpGet("exportar/csv")]
    [Produces("text/csv")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportarCsv([FromQuery] string? termo, [FromQuery] int? ano)
    {
        var csvBytes = await _pesquisaService.ExportarCsvAsync(termo, ano);
        return File(csvBytes, "text/csv", $"transparencia_pe_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
