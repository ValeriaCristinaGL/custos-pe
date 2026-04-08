namespace TransparenciaPE.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEmpenhoRepository Empenhos { get; }
    IContratoRepository Contratos { get; }
    Task<int> CommitAsync();
}
