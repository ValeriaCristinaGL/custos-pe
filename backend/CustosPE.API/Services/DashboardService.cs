using CustosPE.API.Domain.DTOs;
using CustosPE.API.Infrastructure.Data;
using CustosPE.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustosPE.API.Services;

public class DashboardService : IDashboardService
{
    private static readonly string[] NomesMeses =
    [
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];

    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDTO> GetDashboardAsync(int ano)
    {
        var totalDespesas = await _context.Despesas
            .Where(d => d.Ano == ano)
            .SumAsync(d => d.ValorPago);

        var totalReceitas = await _context.Receitas
            .Where(r => r.Ano == ano)
            .SumAsync(r => r.ValorArrecadado);

        var totalOrgaos = await _context.Orgaos.CountAsync();

        return new DashboardDTO
        {
            AnoReferencia = ano,
            TotalDespesas = totalDespesas,
            TotalReceitas = totalReceitas,
            TotalOrgaos = totalOrgaos,
            DespesasPorCategoria = (await GetDespesasPorCategoriaAsync(ano)).ToList(),
            ReceitasPorCategoria = (await GetReceitasPorCategoriaAsync(ano)).ToList(),
            TopOrgaosPorDespesa = (await GetTopOrgaosPorDespesaAsync(ano)).ToList(),
            EvolucaoMensal = (await GetEvolucaoMensalAsync(ano)).ToList()
        };
    }

    public async Task<IEnumerable<DespesaPorCategoriaDTO>> GetDespesasPorCategoriaAsync(int ano)
    {
        var totalGeral = await _context.Despesas
            .Where(d => d.Ano == ano)
            .SumAsync(d => d.ValorPago);

        if (totalGeral == 0) return [];

        var grupos = await _context.Despesas
            .Where(d => d.Ano == ano)
            .GroupBy(d => d.Categoria)
            .Select(g => new { Categoria = g.Key, Total = g.Sum(d => d.ValorPago) })
            .OrderByDescending(g => g.Total)
            .ToListAsync();

        return grupos.Select(g => new DespesaPorCategoriaDTO
        {
            Categoria = g.Categoria,
            ValorTotal = g.Total,
            Percentual = Math.Round(g.Total / totalGeral * 100, 2)
        });
    }

    public async Task<IEnumerable<ReceitaPorCategoriaDTO>> GetReceitasPorCategoriaAsync(int ano)
    {
        var totalGeral = await _context.Receitas
            .Where(r => r.Ano == ano)
            .SumAsync(r => r.ValorArrecadado);

        if (totalGeral == 0) return [];

        var grupos = await _context.Receitas
            .Where(r => r.Ano == ano)
            .GroupBy(r => r.Categoria)
            .Select(g => new { Categoria = g.Key, Total = g.Sum(r => r.ValorArrecadado) })
            .OrderByDescending(g => g.Total)
            .ToListAsync();

        return grupos.Select(g => new ReceitaPorCategoriaDTO
        {
            Categoria = g.Categoria,
            ValorTotal = g.Total,
            Percentual = Math.Round(g.Total / totalGeral * 100, 2)
        });
    }

    public async Task<IEnumerable<DespesaPorOrgaoDTO>> GetTopOrgaosPorDespesaAsync(int ano, int top = 10)
    {
        return await _context.Despesas
            .Where(d => d.Ano == ano)
            .GroupBy(d => new { d.OrgaoId, d.Orgao.Nome, d.Orgao.Sigla })
            .Select(g => new DespesaPorOrgaoDTO
            {
                NomeOrgao = g.Key.Nome,
                SiglaOrgao = g.Key.Sigla ?? g.Key.Nome,
                ValorTotal = g.Sum(d => d.ValorPago)
            })
            .OrderByDescending(d => d.ValorTotal)
            .Take(top)
            .ToListAsync();
    }

    public async Task<IEnumerable<EvolucaoMensalDTO>> GetEvolucaoMensalAsync(int ano)
    {
        var despesasPorMes = await _context.Despesas
            .Where(d => d.Ano == ano)
            .GroupBy(d => d.Mes)
            .Select(g => new { Mes = g.Key, Total = g.Sum(d => d.ValorPago) })
            .ToListAsync();

        var receitasPorMes = await _context.Receitas
            .Where(r => r.Ano == ano)
            .GroupBy(r => r.Mes)
            .Select(g => new { Mes = g.Key, Total = g.Sum(r => r.ValorArrecadado) })
            .ToListAsync();

        return Enumerable.Range(1, 12).Select(mes => new EvolucaoMensalDTO
        {
            Mes = mes,
            NomeMes = NomesMeses[mes - 1],
            TotalDespesas = despesasPorMes.FirstOrDefault(d => d.Mes == mes)?.Total ?? 0,
            TotalReceitas = receitasPorMes.FirstOrDefault(r => r.Mes == mes)?.Total ?? 0
        });
    }
}
