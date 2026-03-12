using CustosPE.API.Domain.DTOs;
using CustosPE.API.Domain.Entities;
using CustosPE.API.Infrastructure.Data;
using CustosPE.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustosPE.API.Services;

public class DespesaService : IDespesaService
{
    private readonly AppDbContext _context;

    public DespesaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DespesaDTO>> GetAllAsync(int? ano = null, int? mes = null, int? orgaoId = null)
    {
        var query = _context.Despesas.Include(d => d.Orgao).AsQueryable();

        if (ano.HasValue) query = query.Where(d => d.Ano == ano.Value);
        if (mes.HasValue) query = query.Where(d => d.Mes == mes.Value);
        if (orgaoId.HasValue) query = query.Where(d => d.OrgaoId == orgaoId.Value);

        return await query.Select(d => ToDTO(d)).ToListAsync();
    }

    public async Task<DespesaDTO?> GetByIdAsync(int id)
    {
        var despesa = await _context.Despesas.Include(d => d.Orgao).FirstOrDefaultAsync(d => d.Id == id);
        return despesa == null ? null : ToDTO(despesa);
    }

    public async Task<IEnumerable<DespesaDTO>> GetByOrgaoAsync(int orgaoId, int? ano = null)
    {
        var query = _context.Despesas.Include(d => d.Orgao).Where(d => d.OrgaoId == orgaoId);
        if (ano.HasValue) query = query.Where(d => d.Ano == ano.Value);
        return await query.Select(d => ToDTO(d)).ToListAsync();
    }

    public async Task<IEnumerable<DespesaDTO>> GetByAnoAsync(int ano)
    {
        return await _context.Despesas.Include(d => d.Orgao)
            .Where(d => d.Ano == ano)
            .Select(d => ToDTO(d))
            .ToListAsync();
    }

    public async Task<DespesaDTO> CreateAsync(CreateDespesaDTO dto)
    {
        var despesa = new Despesa
        {
            OrgaoId = dto.OrgaoId,
            Ano = dto.Ano,
            Mes = dto.Mes,
            ValorEmpenhado = dto.ValorEmpenhado,
            ValorLiquidado = dto.ValorLiquidado,
            ValorPago = dto.ValorPago,
            Categoria = dto.Categoria,
            Funcao = dto.Funcao,
            Subfuncao = dto.Subfuncao,
            Descricao = dto.Descricao,
            CriadoEm = DateTime.UtcNow
        };

        _context.Despesas.Add(despesa);
        await _context.SaveChangesAsync();
        await _context.Entry(despesa).Reference(d => d.Orgao).LoadAsync();

        return ToDTO(despesa);
    }

    public async Task<DespesaDTO?> UpdateAsync(int id, CreateDespesaDTO dto)
    {
        var despesa = await _context.Despesas.Include(d => d.Orgao).FirstOrDefaultAsync(d => d.Id == id);
        if (despesa == null) return null;

        despesa.OrgaoId = dto.OrgaoId;
        despesa.Ano = dto.Ano;
        despesa.Mes = dto.Mes;
        despesa.ValorEmpenhado = dto.ValorEmpenhado;
        despesa.ValorLiquidado = dto.ValorLiquidado;
        despesa.ValorPago = dto.ValorPago;
        despesa.Categoria = dto.Categoria;
        despesa.Funcao = dto.Funcao;
        despesa.Subfuncao = dto.Subfuncao;
        despesa.Descricao = dto.Descricao;

        await _context.SaveChangesAsync();
        await _context.Entry(despesa).Reference(d => d.Orgao).LoadAsync();

        return ToDTO(despesa);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var despesa = await _context.Despesas.FindAsync(id);
        if (despesa == null) return false;

        _context.Despesas.Remove(despesa);
        await _context.SaveChangesAsync();
        return true;
    }

    private static DespesaDTO ToDTO(Despesa d) => new()
    {
        Id = d.Id,
        OrgaoId = d.OrgaoId,
        NomeOrgao = d.Orgao?.Nome ?? string.Empty,
        Ano = d.Ano,
        Mes = d.Mes,
        ValorEmpenhado = d.ValorEmpenhado,
        ValorLiquidado = d.ValorLiquidado,
        ValorPago = d.ValorPago,
        Categoria = d.Categoria,
        Funcao = d.Funcao,
        Subfuncao = d.Subfuncao,
        Descricao = d.Descricao
    };
}
