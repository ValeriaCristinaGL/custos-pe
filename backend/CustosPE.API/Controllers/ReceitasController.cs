using CustosPE.API.Domain.DTOs;
using CustosPE.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustosPE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceitasController : ControllerBase
{
    private readonly IReceitaService _service;

    public ReceitasController(IReceitaService service)
    {
        _service = service;
    }

    /// <summary>Lista receitas com filtros opcionais por ano, mês e órgão.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReceitaDTO>>> GetAll(
        [FromQuery] int? ano,
        [FromQuery] int? mes,
        [FromQuery] int? orgaoId)
    {
        var receitas = await _service.GetAllAsync(ano, mes, orgaoId);
        return Ok(receitas);
    }

    /// <summary>Obtém uma receita pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReceitaDTO>> GetById(int id)
    {
        var receita = await _service.GetByIdAsync(id);
        if (receita == null) return NotFound();
        return Ok(receita);
    }

    /// <summary>Lista receitas de um órgão específico.</summary>
    [HttpGet("orgao/{orgaoId:int}")]
    public async Task<ActionResult<IEnumerable<ReceitaDTO>>> GetByOrgao(int orgaoId, [FromQuery] int? ano)
    {
        var receitas = await _service.GetByOrgaoAsync(orgaoId, ano);
        return Ok(receitas);
    }

    /// <summary>Lista receitas de um ano específico.</summary>
    [HttpGet("ano/{ano:int}")]
    public async Task<ActionResult<IEnumerable<ReceitaDTO>>> GetByAno(int ano)
    {
        var receitas = await _service.GetByAnoAsync(ano);
        return Ok(receitas);
    }

    /// <summary>Cria uma nova receita.</summary>
    [HttpPost]
    public async Task<ActionResult<ReceitaDTO>> Create([FromBody] CreateReceitaDTO dto)
    {
        var receita = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = receita.Id }, receita);
    }

    /// <summary>Atualiza uma receita existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReceitaDTO>> Update(int id, [FromBody] CreateReceitaDTO dto)
    {
        var receita = await _service.UpdateAsync(id, dto);
        if (receita == null) return NotFound();
        return Ok(receita);
    }

    /// <summary>Remove uma receita.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
