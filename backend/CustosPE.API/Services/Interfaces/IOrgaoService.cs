using CustosPE.API.Domain.DTOs;
using CustosPE.API.Domain.Entities;

namespace CustosPE.API.Services.Interfaces;

public interface IOrgaoService
{
    Task<IEnumerable<OrgaoDTO>> GetAllAsync();
    Task<OrgaoDTO?> GetByIdAsync(int id);
    Task<OrgaoDTO> CreateAsync(CreateOrgaoDTO dto);
    Task<OrgaoDTO?> UpdateAsync(int id, CreateOrgaoDTO dto);
    Task<bool> DeleteAsync(int id);
}

public class OrgaoDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Sigla { get; set; }
}

public class CreateOrgaoDTO
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Sigla { get; set; }
}
