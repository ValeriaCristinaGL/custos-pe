using TransparenciaPE.Application.DTOs;

namespace TransparenciaPE.Application.Interfaces;

public interface IPesquisaService
{
    Task<PesquisaResultDto> PesquisaGlobalAsync(string termo);
    Task<byte[]> ExportarCsvAsync(string? termo = null, int? ano = null);
}
