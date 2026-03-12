using CustosPE.API.Domain.Entities;
using CustosPE.API.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CustosPE.API.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Orgao> Orgaos { get; set; }
    public DbSet<Despesa> Despesas { get; set; }
    public DbSet<Receita> Receitas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrgaoConfiguration());
        modelBuilder.ApplyConfiguration(new DespesaConfiguration());
        modelBuilder.ApplyConfiguration(new ReceitaConfiguration());
    }
}
