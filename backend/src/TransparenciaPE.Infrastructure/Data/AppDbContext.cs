using Microsoft.EntityFrameworkCore;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Empenho> Empenhos => Set<Empenho>();
    public DbSet<Liquidacao> Liquidacoes => Set<Liquidacao>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<OrgaoGoverno> OrgaosGoverno => Set<OrgaoGoverno>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
