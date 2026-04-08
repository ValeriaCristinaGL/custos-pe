using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.Domain.Interfaces;

public interface IContratoRepository : IRepository<Contrato>
{
    Task<Contrato?> GetByNumeroAsync(string numeroContrato);
    Task<IEnumerable<Contrato>> SearchByFornecedorAsync(string termo);
    Task<IEnumerable<Contrato>> SearchByCnpjAsync(string cnpj);
}
