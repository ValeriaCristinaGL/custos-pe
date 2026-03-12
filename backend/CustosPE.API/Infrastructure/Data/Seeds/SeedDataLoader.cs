using System.Text.Json;
using System.Text.Json.Serialization;
using CustosPE.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustosPE.API.Infrastructure.Data.Seeds;

public static class SeedDataLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedOrgaosAsync(context);
        await SeedDespesasAsync(context);
        await SeedReceitasAsync(context);
    }

    private static async Task SeedOrgaosAsync(AppDbContext context)
    {
        if (await context.Orgaos.AnyAsync())
            return;

        var json = await ReadSeedFileAsync("orgaos_pe.json");
        var orgaos = JsonSerializer.Deserialize<List<OrgaoSeed>>(json, JsonOptions);
        if (orgaos == null) return;

        var entities = orgaos.Select(o => new Orgao
        {
            Id = o.Id,
            Codigo = o.Codigo,
            Nome = o.Nome,
            Tipo = o.Tipo,
            Sigla = o.Sigla,
            CriadoEm = DateTime.UtcNow
        }).ToList();

        await context.Orgaos.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDespesasAsync(AppDbContext context)
    {
        if (await context.Despesas.AnyAsync())
            return;

        var json = await ReadSeedFileAsync("despesas_2024.json");
        var despesas = JsonSerializer.Deserialize<List<DespesaSeed>>(json, JsonOptions);
        if (despesas == null) return;

        var entities = despesas.Select(d => new Despesa
        {
            Id = d.Id,
            OrgaoId = d.OrgaoId,
            Ano = d.Ano,
            Mes = d.Mes,
            ValorEmpenhado = d.ValorEmpenhado,
            ValorLiquidado = d.ValorLiquidado,
            ValorPago = d.ValorPago,
            Categoria = d.Categoria,
            Funcao = d.Funcao,
            Subfuncao = d.Subfuncao,
            Descricao = d.Descricao,
            CriadoEm = DateTime.UtcNow
        }).ToList();

        await context.Despesas.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }

    private static async Task SeedReceitasAsync(AppDbContext context)
    {
        if (await context.Receitas.AnyAsync())
            return;

        var json = await ReadSeedFileAsync("receitas_2024.json");
        var receitas = JsonSerializer.Deserialize<List<ReceitaSeed>>(json, JsonOptions);
        if (receitas == null) return;

        var entities = receitas.Select(r => new Receita
        {
            Id = r.Id,
            OrgaoId = r.OrgaoId,
            Ano = r.Ano,
            Mes = r.Mes,
            ValorArrecadado = r.ValorArrecadado,
            ValorPrevisto = r.ValorPrevisto,
            Categoria = r.Categoria,
            Fonte = r.Fonte,
            Descricao = r.Descricao,
            CriadoEm = DateTime.UtcNow
        }).ToList();

        await context.Receitas.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }

    private static async Task<string> ReadSeedFileAsync(string fileName)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var seedPath = Path.Combine(basePath, "Infrastructure", "Data", "Seeds", fileName);

        if (!File.Exists(seedPath))
        {
            var altPath = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "Data", "Seeds", fileName);
            if (File.Exists(altPath))
                seedPath = altPath;
            else
                throw new FileNotFoundException($"Seed file not found: {fileName}");
        }

        return await File.ReadAllTextAsync(seedPath);
    }

    private record OrgaoSeed(int Id, string Codigo, string Nome, string Tipo, string? Sigla);

    private record DespesaSeed(
        int Id, int OrgaoId, int Ano, int Mes,
        decimal ValorEmpenhado, decimal ValorLiquidado, decimal ValorPago,
        string Categoria, string Funcao, string? Subfuncao, string? Descricao);

    private record ReceitaSeed(
        int Id, int OrgaoId, int Ano, int Mes,
        decimal ValorArrecadado, decimal ValorPrevisto,
        string Categoria, string Fonte, string? Descricao);
}
