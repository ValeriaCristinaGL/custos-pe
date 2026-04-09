using System.Text;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Helpers;
using TransparenciaPE.Application.Interfaces;
using TransparenciaPE.Domain.Entities;
using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.Application.Services;

public class PesquisaService : IPesquisaService
{
    private readonly IEmpenhoRepository _empenhoRepository;
    private readonly IContratoRepository _contratoRepository;
    private readonly ILogger<PesquisaService> _logger;

    public PesquisaService(
        IEmpenhoRepository empenhoRepository,
        IContratoRepository contratoRepository,
        ILogger<PesquisaService> logger)
    {
        _empenhoRepository = empenhoRepository;
        _contratoRepository = contratoRepository;
        _logger = logger;
    }

    public async Task<PesquisaResultDto> PesquisaGlobalAsync(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            throw new ArgumentException("O termo de busca é obrigatório.", nameof(termo));

        _logger.LogInformation("Global search for term: {Termo}", termo);

        var resultados = new List<PesquisaItem>();

        // Detect if term looks like a CNPJ
        var sanitizedTermo = CnpjHelper.Sanitize(termo);
        var isCnpj = sanitizedTermo.Length == 14 && sanitizedTermo.All(char.IsDigit);

        if (isCnpj)
        {
            var contratos = await _contratoRepository.SearchByCnpjAsync(sanitizedTermo);
            resultados.AddRange(MapContratosToItems(contratos));

            var empenhos = await _empenhoRepository.FindAsync(e => e.CnpjCredor == sanitizedTermo);
            resultados.AddRange(MapEmpenhosToItems(empenhos));
        }
        else
        {
            var contratos = await _contratoRepository.SearchByFornecedorAsync(termo);
            resultados.AddRange(MapContratosToItems(contratos));

            var empenhos = await _empenhoRepository.FindAsync(e =>
                e.Credor.Contains(termo) || e.NumeroEmpenho.Contains(termo));
            resultados.AddRange(MapEmpenhosToItems(empenhos));
        }

        return new PesquisaResultDto
        {
            Resultados = resultados,
            TotalResultados = resultados.Count,
            TermoBuscado = termo
        };
    }

    public async Task<byte[]> ExportarCsvAsync(string? termo = null, int? ano = null)
    {
        _logger.LogInformation("Exporting CSV - Term: {Termo}, Year: {Ano}", termo ?? "all", ano?.ToString() ?? "all");

        var empenhos = await _empenhoRepository.FindAsync(e =>
            (ano == null || e.Ano == ano) &&
            (termo == null || e.Credor.Contains(termo) || e.CnpjCredor.Contains(termo)));

        var sb = new StringBuilder();
        sb.AppendLine("NumeroEmpenho;Orgao;Credor;CNPJ;Valor;Data;Descricao");

        foreach (var e in empenhos)
        {
            sb.AppendLine($"{e.NumeroEmpenho};{e.OrgaoGoverno?.Nome};{e.Credor};{e.CnpjCredor};{e.Valor};{e.DataEmpenho:yyyy-MM-dd};{e.Descricao}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static IEnumerable<PesquisaItem> MapContratosToItems(IEnumerable<Contrato> contratos)
    {
        return contratos.Select(c => new PesquisaItem
        {
            Tipo = "Contrato",
            Numero = c.NumeroContrato,
            OrgaoNome = c.OrgaoGoverno?.Nome ?? string.Empty,
            Fornecedor = c.Fornecedor,
            Cnpj = c.CnpjFornecedor,
            Valor = c.ValorContrato,
            Data = c.DataInicio,
            Descricao = c.Objeto
        });
    }

    private static IEnumerable<PesquisaItem> MapEmpenhosToItems(IEnumerable<Empenho> empenhos)
    {
        return empenhos.Select(e => new PesquisaItem
        {
            Tipo = "Empenho",
            Numero = e.NumeroEmpenho,
            OrgaoNome = e.OrgaoGoverno?.Nome ?? string.Empty,
            Fornecedor = e.Credor,
            Cnpj = e.CnpjCredor,
            Valor = e.Valor,
            Data = e.DataEmpenho,
            Descricao = e.Descricao
        });
    }
}
