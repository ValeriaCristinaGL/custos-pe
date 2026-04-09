using Microsoft.EntityFrameworkCore;
using TransparenciaPE.Domain.Entities;
using TransparenciaPE.Domain.Interfaces;
using TransparenciaPE.Infrastructure.Data;

namespace TransparenciaPE.Infrastructure.Repositories;

public class EmpenhoRepository : Repository<Empenho>, IEmpenhoRepository
{
    public EmpenhoRepository(AppDbContext context) : base(context) { }

    public async Task<Empenho?> GetByNumeroAsync(string numeroEmpenho, int ano)
        => await _dbSet
            .Include(e => e.OrgaoGoverno)
            .FirstOrDefaultAsync(e => e.NumeroEmpenho == numeroEmpenho && e.Ano == ano);

    public async Task<IEnumerable<Empenho>> GetByOrgaoAsync(Guid orgaoId)
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.OrgaoGoverno)
            .Where(e => e.OrgaoGovernoId == orgaoId)
            .ToListAsync();

    public async Task<IEnumerable<Empenho>> GetByAnoAsync(int ano)
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.OrgaoGoverno)
            .Where(e => e.Ano == ano)
            .ToListAsync();
}

public class ContratoRepository : Repository<Contrato>, IContratoRepository
{
    public ContratoRepository(AppDbContext context) : base(context) { }

    public async Task<Contrato?> GetByNumeroAsync(string numeroContrato)
        => await _dbSet
            .Include(c => c.OrgaoGoverno)
            .FirstOrDefaultAsync(c => c.NumeroContrato == numeroContrato);

    public async Task<IEnumerable<Contrato>> SearchByFornecedorAsync(string termo)
        => await _dbSet
            .AsNoTracking()
            .Include(c => c.OrgaoGoverno)
            .Where(c => EF.Functions.ILike(c.Fornecedor, $"%{termo}%"))
            .ToListAsync();

    public async Task<IEnumerable<Contrato>> SearchByCnpjAsync(string cnpj)
        => await _dbSet
            .AsNoTracking()
            .Include(c => c.OrgaoGoverno)
            .Where(c => c.CnpjFornecedor == cnpj)
            .ToListAsync();
}
