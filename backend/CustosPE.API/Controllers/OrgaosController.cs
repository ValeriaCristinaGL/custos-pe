using CustosPE.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustosPE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrgaosController : ControllerBase
{
    private readonly IOrgaoService _service;

    public OrgaosController(IOrgaoService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os órgãos do estado.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrgaoDTO>>> GetAll()
    {
        var orgaos = await _service.GetAllAsync();
        return Ok(orgaos);
    }

    /// <summary>Obtém um órgão pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrgaoDTO>> GetById(int id)
    {
        var orgao = await _service.GetByIdAsync(id);
        if (orgao == null) return NotFound();
        return Ok(orgao);
    }

    /// <summary>Cria um novo órgão.</summary>
    [HttpPost]
    public async Task<ActionResult<OrgaoDTO>> Create([FromBody] CreateOrgaoDTO dto)
    {
        var orgao = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = orgao.Id }, orgao);
    }

    /// <summary>Atualiza um órgão existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrgaoDTO>> Update(int id, [FromBody] CreateOrgaoDTO dto)
    {
        var orgao = await _service.UpdateAsync(id, dto);
        if (orgao == null) return NotFound();
        return Ok(orgao);
    }

    /// <summary>Remove um órgão.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
