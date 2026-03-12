using CustosPE.API.Domain.DTOs;

namespace CustosPE.API.Services.Interfaces;

public interface IReceitaService
{
    Task<IEnumerable<ReceitaDTO>> GetAllAsync(int? ano = null, int? mes = null, int? orgaoId = null);
    Task<ReceitaDTO?> GetByIdAsync(int id);
    Task<IEnumerable<ReceitaDTO>> GetByOrgaoAsync(int orgaoId, int? ano = null);
    Task<IEnumerable<ReceitaDTO>> GetByAnoAsync(int ano);
    Task<ReceitaDTO> CreateAsync(CreateReceitaDTO dto);
    Task<ReceitaDTO?> UpdateAsync(int id, CreateReceitaDTO dto);
    Task<bool> DeleteAsync(int id);
}
