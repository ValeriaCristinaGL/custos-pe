using CustosPE.API.Domain.DTOs;
using CustosPE.API.Domain.Entities;
using CustosPE.API.Infrastructure.Data;
using CustosPE.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustosPE.API.Services;

public class ReceitaService : IReceitaService
{
    private readonly AppDbContext _context;

    public ReceitaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReceitaDTO>> GetAllAsync(int? ano = null, int? mes = null, int? orgaoId = null)
    {
        var query = _context.Receitas.Include(r => r.Orgao).AsQueryable();

        if (ano.HasValue) query = query.Where(r => r.Ano == ano.Value);
        if (mes.HasValue) query = query.Where(r => r.Mes == mes.Value);
        if (orgaoId.HasValue) query = query.Where(r => r.OrgaoId == orgaoId.Value);

        return await query.Select(r => ToDTO(r)).ToListAsync();
    }

    public async Task<ReceitaDTO?> GetByIdAsync(int id)
    {
        var receita = await _context.Receitas.Include(r => r.Orgao).FirstOrDefaultAsync(r => r.Id == id);
        return receita == null ? null : ToDTO(receita);
    }

    public async Task<IEnumerable<ReceitaDTO>> GetByOrgaoAsync(int orgaoId, int? ano = null)
    {
        var query = _context.Receitas.Include(r => r.Orgao).Where(r => r.OrgaoId == orgaoId);
        if (ano.HasValue) query = query.Where(r => r.Ano == ano.Value);
        return await query.Select(r => ToDTO(r)).ToListAsync();
    }

    public async Task<IEnumerable<ReceitaDTO>> GetByAnoAsync(int ano)
    {
        return await _context.Receitas.Include(r => r.Orgao)
            .Where(r => r.Ano == ano)
            .Select(r => ToDTO(r))
            .ToListAsync();
    }

    public async Task<ReceitaDTO> CreateAsync(CreateReceitaDTO dto)
    {
        var receita = new Receita
        {
            OrgaoId = dto.OrgaoId,
            Ano = dto.Ano,
            Mes = dto.Mes,
            ValorArrecadado = dto.ValorArrecadado,
            ValorPrevisto = dto.ValorPrevisto,
            Categoria = dto.Categoria,
            Fonte = dto.Fonte,
            Descricao = dto.Descricao,
            CriadoEm = DateTime.UtcNow
        };

        _context.Receitas.Add(receita);
        await _context.SaveChangesAsync();
        await _context.Entry(receita).Reference(r => r.Orgao).LoadAsync();

        return ToDTO(receita);
    }

    public async Task<ReceitaDTO?> UpdateAsync(int id, CreateReceitaDTO dto)
    {
        var receita = await _context.Receitas.Include(r => r.Orgao).FirstOrDefaultAsync(r => r.Id == id);
        if (receita == null) return null;

        receita.OrgaoId = dto.OrgaoId;
        receita.Ano = dto.Ano;
        receita.Mes = dto.Mes;
        receita.ValorArrecadado = dto.ValorArrecadado;
        receita.ValorPrevisto = dto.ValorPrevisto;
        receita.Categoria = dto.Categoria;
        receita.Fonte = dto.Fonte;
        receita.Descricao = dto.Descricao;

        await _context.SaveChangesAsync();
        await _context.Entry(receita).Reference(r => r.Orgao).LoadAsync();

        return ToDTO(receita);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var receita = await _context.Receitas.FindAsync(id);
        if (receita == null) return false;

        _context.Receitas.Remove(receita);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ReceitaDTO ToDTO(Receita r) => new()
    {
        Id = r.Id,
        OrgaoId = r.OrgaoId,
        NomeOrgao = r.Orgao?.Nome ?? string.Empty,
        Ano = r.Ano,
        Mes = r.Mes,
        ValorArrecadado = r.ValorArrecadado,
        ValorPrevisto = r.ValorPrevisto,
        Categoria = r.Categoria,
        Fonte = r.Fonte,
        Descricao = r.Descricao
    };
}
