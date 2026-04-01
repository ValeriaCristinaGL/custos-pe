using TransparenciaPE.Domain.Interfaces;
using TransparenciaPE.Infrastructure.Data;

namespace TransparenciaPE.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        IEmpenhoRepository empenhoRepository,
        IContratoRepository contratoRepository)
    {
        _context = context;
        Empenhos = empenhoRepository;
        Contratos = contratoRepository;
    }

    public IEmpenhoRepository Empenhos { get; }
    public IContratoRepository Contratos { get; }

    public async Task<int> CommitAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}
