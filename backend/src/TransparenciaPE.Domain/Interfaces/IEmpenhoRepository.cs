using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.Domain.Interfaces;

public interface IEmpenhoRepository : IRepository<Empenho>
{
    Task<Empenho?> GetByNumeroAsync(string numeroEmpenho, int ano);
    Task<IEnumerable<Empenho>> GetByOrgaoAsync(Guid orgaoId);
    Task<IEnumerable<Empenho>> GetByAnoAsync(int ano);
}
