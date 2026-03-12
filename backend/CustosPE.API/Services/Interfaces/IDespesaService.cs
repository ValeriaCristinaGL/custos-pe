using CustosPE.API.Domain.DTOs;

namespace CustosPE.API.Services.Interfaces;

public interface IDespesaService
{
    Task<IEnumerable<DespesaDTO>> GetAllAsync(int? ano = null, int? mes = null, int? orgaoId = null);
    Task<DespesaDTO?> GetByIdAsync(int id);
    Task<IEnumerable<DespesaDTO>> GetByOrgaoAsync(int orgaoId, int? ano = null);
    Task<IEnumerable<DespesaDTO>> GetByAnoAsync(int ano);
    Task<DespesaDTO> CreateAsync(CreateDespesaDTO dto);
    Task<DespesaDTO?> UpdateAsync(int id, CreateDespesaDTO dto);
    Task<bool> DeleteAsync(int id);
}
