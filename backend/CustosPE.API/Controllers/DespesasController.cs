using CustosPE.API.Domain.DTOs;
using CustosPE.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustosPE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DespesasController : ControllerBase
{
    private readonly IDespesaService _service;

    public DespesasController(IDespesaService service)
    {
        _service = service;
    }

    /// <summary>Lista despesas com filtros opcionais por ano, mês e órgão.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DespesaDTO>>> GetAll(
        [FromQuery] int? ano,
        [FromQuery] int? mes,
        [FromQuery] int? orgaoId)
    {
        var despesas = await _service.GetAllAsync(ano, mes, orgaoId);
        return Ok(despesas);
    }

    /// <summary>Obtém uma despesa pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DespesaDTO>> GetById(int id)
    {
        var despesa = await _service.GetByIdAsync(id);
        if (despesa == null) return NotFound();
        return Ok(despesa);
    }

    /// <summary>Lista despesas de um órgão específico.</summary>
    [HttpGet("orgao/{orgaoId:int}")]
    public async Task<ActionResult<IEnumerable<DespesaDTO>>> GetByOrgao(int orgaoId, [FromQuery] int? ano)
    {
        var despesas = await _service.GetByOrgaoAsync(orgaoId, ano);
        return Ok(despesas);
    }

    /// <summary>Lista despesas de um ano específico.</summary>
    [HttpGet("ano/{ano:int}")]
    public async Task<ActionResult<IEnumerable<DespesaDTO>>> GetByAno(int ano)
    {
        var despesas = await _service.GetByAnoAsync(ano);
        return Ok(despesas);
    }

    /// <summary>Cria uma nova despesa.</summary>
    [HttpPost]
    public async Task<ActionResult<DespesaDTO>> Create([FromBody] CreateDespesaDTO dto)
    {
        var despesa = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = despesa.Id }, despesa);
    }

    /// <summary>Atualiza uma despesa existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<DespesaDTO>> Update(int id, [FromBody] CreateDespesaDTO dto)
    {
        var despesa = await _service.UpdateAsync(id, dto);
        if (despesa == null) return NotFound();
        return Ok(despesa);
    }

    /// <summary>Remove uma despesa.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
