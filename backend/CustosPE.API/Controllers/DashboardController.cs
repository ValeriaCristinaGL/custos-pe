using CustosPE.API.Domain.DTOs;
using CustosPE.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustosPE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    /// <summary>Retorna dados agregados do dashboard para um ano específico.</summary>
    [HttpGet]
    public async Task<ActionResult<DashboardDTO>> GetDashboard([FromQuery] int ano = 2024)
    {
        var dashboard = await _service.GetDashboardAsync(ano);
        return Ok(dashboard);
    }

    /// <summary>Retorna despesas agrupadas por categoria.</summary>
    [HttpGet("despesas/categorias")]
    public async Task<ActionResult<IEnumerable<DespesaPorCategoriaDTO>>> GetDespesasPorCategoria([FromQuery] int ano = 2024)
    {
        var dados = await _service.GetDespesasPorCategoriaAsync(ano);
        return Ok(dados);
    }

    /// <summary>Retorna receitas agrupadas por categoria.</summary>
    [HttpGet("receitas/categorias")]
    public async Task<ActionResult<IEnumerable<ReceitaPorCategoriaDTO>>> GetReceitasPorCategoria([FromQuery] int ano = 2024)
    {
        var dados = await _service.GetReceitasPorCategoriaAsync(ano);
        return Ok(dados);
    }

    /// <summary>Retorna os órgãos com maiores despesas no ano.</summary>
    [HttpGet("despesas/top-orgaos")]
    public async Task<ActionResult<IEnumerable<DespesaPorOrgaoDTO>>> GetTopOrgaosPorDespesa(
        [FromQuery] int ano = 2024,
        [FromQuery] int top = 10)
    {
        var dados = await _service.GetTopOrgaosPorDespesaAsync(ano, top);
        return Ok(dados);
    }

    /// <summary>Retorna a evolução mensal de despesas e receitas no ano.</summary>
    [HttpGet("evolucao-mensal")]
    public async Task<ActionResult<IEnumerable<EvolucaoMensalDTO>>> GetEvolucaoMensal([FromQuery] int ano = 2024)
    {
        var dados = await _service.GetEvolucaoMensalAsync(ano);
        return Ok(dados);
    }
}
