using CustosPE.API.Domain.Entities;
using CustosPE.API.Infrastructure.Data;
using CustosPE.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustosPE.API.Services;

public class OrgaoService : IOrgaoService
{
    private readonly AppDbContext _context;

    public OrgaoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrgaoDTO>> GetAllAsync()
    {
        return await _context.Orgaos
            .Select(o => ToDTO(o))
            .ToListAsync();
    }

    public async Task<OrgaoDTO?> GetByIdAsync(int id)
    {
        var orgao = await _context.Orgaos.FindAsync(id);
        return orgao == null ? null : ToDTO(orgao);
    }

    public async Task<OrgaoDTO> CreateAsync(CreateOrgaoDTO dto)
    {
        var orgao = new Orgao
        {
            Codigo = dto.Codigo,
            Nome = dto.Nome,
            Tipo = dto.Tipo,
            Sigla = dto.Sigla,
            CriadoEm = DateTime.UtcNow
        };

        _context.Orgaos.Add(orgao);
        await _context.SaveChangesAsync();
        return ToDTO(orgao);
    }

    public async Task<OrgaoDTO?> UpdateAsync(int id, CreateOrgaoDTO dto)
    {
        var orgao = await _context.Orgaos.FindAsync(id);
        if (orgao == null) return null;

        orgao.Codigo = dto.Codigo;
        orgao.Nome = dto.Nome;
        orgao.Tipo = dto.Tipo;
        orgao.Sigla = dto.Sigla;

        await _context.SaveChangesAsync();
        return ToDTO(orgao);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orgao = await _context.Orgaos.FindAsync(id);
        if (orgao == null) return false;

        _context.Orgaos.Remove(orgao);
        await _context.SaveChangesAsync();
        return true;
    }

    private static OrgaoDTO ToDTO(Orgao o) => new()
    {
        Id = o.Id,
        Codigo = o.Codigo,
        Nome = o.Nome,
        Tipo = o.Tipo,
        Sigla = o.Sigla
    };
}
