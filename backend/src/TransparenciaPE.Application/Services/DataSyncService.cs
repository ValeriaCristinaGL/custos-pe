using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.Helpers;
using TransparenciaPE.Application.Interfaces;
using TransparenciaPE.Domain.Entities;
using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.Application.Services;

public class DataSyncService : IDataSyncService
{
    private readonly IPEDataClient _dataClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OrgaoGoverno> _orgaoRepository;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(
        IPEDataClient dataClient,
        IUnitOfWork unitOfWork,
        IRepository<OrgaoGoverno> orgaoRepository,
        ILogger<DataSyncService> logger)
    {
        _dataClient = dataClient;
        _unitOfWork = unitOfWork;
        _orgaoRepository = orgaoRepository;
        _logger = logger;
    }

    public async Task<int> SyncEmpenhosAsync(int ano)
    {
        _logger.LogInformation("Starting empenho sync for year {Ano}", ano);

        var externalData = await _dataClient.GetEmpenhosAsync(ano);
        var count = 0;

        foreach (var item in externalData)
        {
            var orgao = await GetOrCreateOrgaoAsync(item.CodigoOrgao, item.NomeOrgao, item.SiglaOrgao);
            var existing = await _unitOfWork.Empenhos.GetByNumeroAsync(item.NumeroEmpenho, item.Ano);

            if (existing is not null)
            {
                // Upsert: Update existing
                existing.Valor = item.Valor;
                existing.Credor = item.Credor;
                existing.CnpjCredor = CnpjHelper.Sanitize(item.CnpjCredor);
                existing.Descricao = item.Descricao;
                existing.ClassificacaoMcasp = McaspMapper.MapToClassificacao(item.NaturezaDespesa, item.Descricao);
                _logger.LogDebug("Updated empenho {NumeroEmpenho}", item.NumeroEmpenho);
            }
            else
            {
                // Insert new
                var empenho = new Empenho
                {
                    NumeroEmpenho = item.NumeroEmpenho,
                    Ano = item.Ano,
                    OrgaoGovernoId = orgao.Id,
                    Credor = item.Credor,
                    CnpjCredor = CnpjHelper.Sanitize(item.CnpjCredor),
                    Valor = item.Valor,
                    DataEmpenho = DateTime.SpecifyKind(item.DataEmpenho, DateTimeKind.Utc),
                    Descricao = item.Descricao,
                    ClassificacaoMcasp = McaspMapper.MapToClassificacao(item.NaturezaDespesa, item.Descricao)
                };
                await _unitOfWork.Empenhos.AddAsync(empenho);
                _logger.LogDebug("Inserted new empenho {NumeroEmpenho}", item.NumeroEmpenho);
            }

            count++;
        }

        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Synced {Count} empenhos for year {Ano}", count, ano);
        return count;
    }

    public async Task<int> SyncContratosAsync(int ano)
    {
        _logger.LogInformation("Starting contract sync for year {Ano}", ano);

        var externalData = await _dataClient.GetContratosAsync(ano);
        var count = 0;

        foreach (var item in externalData)
        {
            var orgao = await GetOrCreateOrgaoAsync(item.CodigoOrgao, item.NomeOrgao, "");
            var existing = await _unitOfWork.Contratos.GetByNumeroAsync(item.NumeroContrato);

            if (existing is not null)
            {
                existing.ValorContrato = item.ValorContrato;
                existing.Fornecedor = item.Fornecedor;
                existing.CnpjFornecedor = CnpjHelper.Sanitize(item.CnpjFornecedor);
                existing.Objeto = item.Objeto;
                existing.DataFim = item.DataFim;
            }
            else
            {
                var contrato = new Contrato
                {
                    NumeroContrato = item.NumeroContrato,
                    OrgaoGovernoId = orgao.Id,
                    Fornecedor = item.Fornecedor,
                    CnpjFornecedor = CnpjHelper.Sanitize(item.CnpjFornecedor),
                    ValorContrato = item.ValorContrato,
                    DataInicio = DateTime.SpecifyKind(item.DataInicio, DateTimeKind.Utc),
                    DataFim = item.DataFim.HasValue ? DateTime.SpecifyKind(item.DataFim.Value, DateTimeKind.Utc) : null,
                    Objeto = item.Objeto
                };
                await _unitOfWork.Contratos.AddAsync(contrato);
            }

            count++;
        }

        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Synced {Count} contracts for year {Ano}", count, ano);
        return count;
    }

    public async Task<SyncResultDto> SyncAllAsync(int ano)
    {
        var empenhos = await SyncEmpenhosAsync(ano);
        var contratos = await SyncContratosAsync(ano);

        return new SyncResultDto
        {
            EmpenhosProcessados = empenhos,
            ContratosProcessados = contratos,
            SyncedAt = DateTime.UtcNow
        };
    }

    private async Task<OrgaoGoverno> GetOrCreateOrgaoAsync(string codigo, string nome, string sigla)
    {
        var existing = await _orgaoRepository.FindAsync(o => o.Codigo == codigo);
        var orgao = existing.FirstOrDefault();

        if (orgao is not null)
            return orgao;

        orgao = new OrgaoGoverno
        {
            Codigo = codigo,
            Nome = nome,
            Sigla = sigla,
            Tipo = "Secretaria"
        };
        await _orgaoRepository.AddAsync(orgao);
        return orgao;
    }
}
