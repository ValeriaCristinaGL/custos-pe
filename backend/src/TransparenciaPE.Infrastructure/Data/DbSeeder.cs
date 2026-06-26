using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        // Only seed if the database is empty
        if (await context.OrgaosGoverno.AnyAsync())
        {
            logger.LogInformation("Banco de dados já possui dados. Seed ignorado.");
            return;
        }

        logger.LogInformation("Iniciando seed do banco de dados com dados fictícios...");

        // ──────────────────────────────────────────────
        // 1. Órgãos de Governo (baseados em órgãos reais de PE)
        // ──────────────────────────────────────────────
        var orgaos = new List<OrgaoGoverno>
        {
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567801"),
                Codigo = "SEC-EDUCACAO",
                Nome = "Secretaria de Educação e Esportes",
                Sigla = "SEE",
                Tipo = "Secretaria",
                TotalServidores = 45320,
                OrcamentoAtual = 4_850_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567802"),
                Codigo = "SEC-SAUDE",
                Nome = "Secretaria Estadual de Saúde",
                Sigla = "SES",
                Tipo = "Secretaria",
                TotalServidores = 32150,
                OrcamentoAtual = 6_200_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567803"),
                Codigo = "SEC-FAZENDA",
                Nome = "Secretaria da Fazenda",
                Sigla = "SEFAZ",
                Tipo = "Secretaria",
                TotalServidores = 4800,
                OrcamentoAtual = 890_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567804"),
                Codigo = "SEC-INFRA",
                Nome = "Secretaria de Infraestrutura e Recursos Hídricos",
                Sigla = "SEINFRA",
                Tipo = "Secretaria",
                TotalServidores = 2350,
                OrcamentoAtual = 3_100_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567805"),
                Codigo = "SEC-SEGURANCA",
                Nome = "Secretaria de Defesa Social",
                Sigla = "SDS",
                Tipo = "Secretaria",
                TotalServidores = 38500,
                OrcamentoAtual = 4_200_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567806"),
                Codigo = "SEC-ADMIN",
                Nome = "Secretaria de Administração",
                Sigla = "SAD",
                Tipo = "Secretaria",
                TotalServidores = 1850,
                OrcamentoAtual = 520_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567807"),
                Codigo = "SEC-DESENV",
                Nome = "Secretaria de Desenvolvimento Econômico",
                Sigla = "SDEC",
                Tipo = "Secretaria",
                TotalServidores = 1200,
                OrcamentoAtual = 780_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567808"),
                Codigo = "SEC-ASSIST",
                Nome = "Secretaria de Desenvolvimento Social, Criança e Juventude",
                Sigla = "SDSCJ",
                Tipo = "Secretaria",
                TotalServidores = 3100,
                OrcamentoAtual = 1_450_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567809"),
                Codigo = "DETRAN-PE",
                Nome = "Departamento Estadual de Trânsito de Pernambuco",
                Sigla = "DETRAN",
                Tipo = "Autarquia",
                TotalServidores = 2800,
                OrcamentoAtual = 650_000_000.00m
            },
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567810"),
                Codigo = "SEC-CULTURA",
                Nome = "Secretaria de Cultura",
                Sigla = "SECULT",
                Tipo = "Secretaria",
                TotalServidores = 850,
                OrcamentoAtual = 320_000_000.00m
            }
        };

        context.OrgaosGoverno.AddRange(orgaos);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} órgãos de governo inseridos.", orgaos.Count);

        // ──────────────────────────────────────────────
        // 2. Fornecedores fictícios (para empenhos e contratos)
        // ──────────────────────────────────────────────
        var fornecedores = new (string Nome, string Cnpj)[]
        {
            ("Tech Solutions PE Ltda", "12345678000101"),
            ("Construtora Recife Engenharia S.A.", "23456789000102"),
            ("Distribuidora de Medicamentos NordeFarma", "34567890000103"),
            ("Alimentos & Nutrição Capibaribe Ltda", "45678901000104"),
            ("Serviços de Limpeza Olinda Ltda", "56789012000105"),
            ("Segurança Patrimonial Agreste S.A.", "67890123000106"),
            ("Editora e Gráfica Sertão PE", "78901234000107"),
            ("Transportes Metropolitano Recife Ltda", "89012345000108"),
            ("Consultoria DataGov Analytics", "90123456000109"),
            ("Equipamentos Hospitalares MedTech PE", "01234567000110"),
            ("Pavimentação e Terraplanagem Caruaru", "11223344000111"),
            ("Fornecedora de Material Escolar EduPE", "22334455000112"),
            ("Informática e Redes CyberRecife", "33445566000113"),
            ("Manutenção Predial ServiçosTOP", "44556677000114"),
            ("Combustíveis e Lubrificantes PetroNE", "55667788000115")
        };

        // Descrições MCASP
        var classificacoes = new[]
        {
            "3.3.90.30 - Material de Consumo",
            "3.3.90.39 - Outros Serviços de Terceiros - PJ",
            "3.3.90.36 - Outros Serviços de Terceiros - PF",
            "4.4.90.51 - Obras e Instalações",
            "4.4.90.52 - Equipamentos e Material Permanente",
            "3.1.90.11 - Vencimentos e Vantagens Fixas",
            "3.3.90.33 - Passagens e Despesas com Locomoção",
            "3.3.90.14 - Diárias",
            "4.4.90.61 - Aquisição de Imóveis",
            "3.3.90.92 - Despesas de Exercícios Anteriores"
        };

        var descricoes = new[]
        {
            "Aquisição de material didático para escolas estaduais",
            "Prestação de serviços de manutenção predial",
            "Fornecimento de medicamentos e insumos hospitalares",
            "Serviços de vigilância e segurança patrimonial",
            "Aquisição de equipamentos de informática",
            "Serviços de limpeza e conservação",
            "Obras de pavimentação e drenagem urbana",
            "Fornecimento de alimentação escolar (merenda)",
            "Consultoria em tecnologia da informação",
            "Manutenção de frota de veículos oficiais",
            "Aquisição de mobiliário para unidades de saúde",
            "Serviços de transporte escolar",
            "Reforma e ampliação de unidade hospitalar",
            "Impressão de material gráfico institucional",
            "Fornecimento de combustível para frota oficial"
        };

        var random = new Random(42); // Seed fixo para reprodutibilidade
        var anos = new[] { 2023, 2024, 2025 };
        var empenhoCounter = 0;
        var allEmpenhos = new List<Empenho>();

        // ──────────────────────────────────────────────
        // 3. Empenhos (~ 10-15 por órgão, por ano)
        // ──────────────────────────────────────────────
        foreach (var orgao in orgaos)
        {
            foreach (var ano in anos)
            {
                var qtdEmpenhos = random.Next(8, 16);
                for (int i = 0; i < qtdEmpenhos; i++)
                {
                    empenhoCounter++;
                    var fornecedor = fornecedores[random.Next(fornecedores.Length)];
                    var mes = random.Next(1, 13);
                    var dia = random.Next(1, 29);

                    var empenho = new Empenho
                    {
                        NumeroEmpenho = $"{ano}NE{empenhoCounter:D6}",
                        Ano = ano,
                        OrgaoGovernoId = orgao.Id,
                        Credor = fornecedor.Nome,
                        CnpjCredor = fornecedor.Cnpj,
                        Valor = Math.Round((decimal)(random.NextDouble() * 4_500_000 + 50_000), 2),
                        DataEmpenho = new DateTime(ano, mes, dia, 0, 0, 0, DateTimeKind.Utc),
                        Descricao = descricoes[random.Next(descricoes.Length)],
                        ClassificacaoMcasp = classificacoes[random.Next(classificacoes.Length)]
                    };
                    allEmpenhos.Add(empenho);
                }
            }
        }

        context.Empenhos.AddRange(allEmpenhos);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} empenhos inseridos.", allEmpenhos.Count);

        // ──────────────────────────────────────────────
        // 4. Liquidações (1-2 por empenho)
        // ──────────────────────────────────────────────
        var liquidacaoCounter = 0;
        var allLiquidacoes = new List<Liquidacao>();

        foreach (var empenho in allEmpenhos)
        {
            var qtdLiquidacoes = random.Next(1, 3);
            var valorRestante = empenho.Valor;

            for (int i = 0; i < qtdLiquidacoes; i++)
            {
                liquidacaoCounter++;
                var valorLiq = (i == qtdLiquidacoes - 1)
                    ? valorRestante
                    : Math.Round(valorRestante * (decimal)(random.NextDouble() * 0.5 + 0.3), 2);
                valorRestante -= valorLiq;

                var diasAposEmpenho = random.Next(15, 60);
                var dataLiq = empenho.DataEmpenho.AddDays(diasAposEmpenho);
                // Garantir que não ultrapasse o fim do ano seguinte
                if (dataLiq.Year > empenho.Ano + 1)
                    dataLiq = new DateTime(empenho.Ano + 1, 12, 28, 0, 0, 0, DateTimeKind.Utc);

                var liquidacao = new Liquidacao
                {
                    NumeroLiquidacao = $"{empenho.Ano}NL{liquidacaoCounter:D6}",
                    EmpenhoId = empenho.Id,
                    Valor = valorLiq,
                    DataLiquidacao = dataLiq
                };
                allLiquidacoes.Add(liquidacao);
            }
        }

        context.Liquidacoes.AddRange(allLiquidacoes);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} liquidações inseridas.", allLiquidacoes.Count);

        // ──────────────────────────────────────────────
        // 5. Pagamentos (1 por liquidação, ~90% das liquidações são pagas)
        // ──────────────────────────────────────────────
        var pagamentoCounter = 0;
        var allPagamentos = new List<Pagamento>();

        foreach (var liquidacao in allLiquidacoes)
        {
            if (random.NextDouble() > 0.10) // 90% de chance de pagamento
            {
                pagamentoCounter++;
                var diasAposLiquidacao = random.Next(5, 30);
                var dataPag = liquidacao.DataLiquidacao.AddDays(diasAposLiquidacao);

                var pagamento = new Pagamento
                {
                    NumeroPagamento = $"{liquidacao.DataLiquidacao.Year}NP{pagamentoCounter:D6}",
                    LiquidacaoId = liquidacao.Id,
                    Valor = liquidacao.Valor,
                    DataPagamento = dataPag
                };
                allPagamentos.Add(pagamento);
            }
        }

        context.Pagamentos.AddRange(allPagamentos);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} pagamentos inseridos.", allPagamentos.Count);

        // ──────────────────────────────────────────────
        // 6. Contratos (2-5 por órgão)
        // ──────────────────────────────────────────────
        var contratoCounter = 0;
        var allContratos = new List<Contrato>();

        var objetosContrato = new[]
        {
            "Prestação de serviços continuados de limpeza, conservação e higienização",
            "Fornecimento de equipamentos de informática e periféricos",
            "Execução de obras de reforma e ampliação de prédio público",
            "Serviços de vigilância armada e desarmada",
            "Locação de veículos sem motorista para transporte administrativo",
            "Fornecimento de refeições industriais para servidores",
            "Manutenção preventiva e corretiva de ar-condicionado",
            "Serviços de impressão e reprografia corporativa",
            "Consultoria especializada em gestão pública",
            "Fornecimento de material médico-hospitalar",
            "Serviços de desenvolvimento e manutenção de sistemas",
            "Execução de obras de pavimentação asfáltica",
            "Fornecimento de material de expediente e escritório",
            "Serviços de coleta e tratamento de resíduos sólidos",
            "Implantação de sistema de videomonitoramento"
        };

        foreach (var orgao in orgaos)
        {
            var qtdContratos = random.Next(2, 6);
            for (int i = 0; i < qtdContratos; i++)
            {
                contratoCounter++;
                var fornecedor = fornecedores[random.Next(fornecedores.Length)];
                var anoInicio = anos[random.Next(anos.Length)];
                var mesInicio = random.Next(1, 13);
                var dataInicio = new DateTime(anoInicio, mesInicio, random.Next(1, 29), 0, 0, 0, DateTimeKind.Utc);
                var duracaoMeses = random.Next(6, 37); // 6 meses a 3 anos

                var contrato = new Contrato
                {
                    NumeroContrato = $"CT-{anoInicio}/{contratoCounter:D4}",
                    OrgaoGovernoId = orgao.Id,
                    Fornecedor = fornecedor.Nome,
                    CnpjFornecedor = fornecedor.Cnpj,
                    ValorContrato = Math.Round((decimal)(random.NextDouble() * 9_000_000 + 500_000), 2),
                    DataInicio = dataInicio,
                    DataFim = dataInicio.AddMonths(duracaoMeses),
                    Objeto = objetosContrato[random.Next(objetosContrato.Length)]
                };
                allContratos.Add(contrato);
            }
        }

        context.Contratos.AddRange(allContratos);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} contratos inseridos.", allContratos.Count);

        // ──────────────────────────────────────────────
        // 7. Receitas (mensais por órgão, 2023-2025)
        // ──────────────────────────────────────────────
        var origensReceita = new[]
        {
            "Receita Tributária",
            "Transferências da União",
            "Receita de Contribuições",
            "Receita Patrimonial",
            "Receita de Serviços",
            "Transferências do FUNDEB",
            "Receita Industrial",
            "Outras Receitas Correntes"
        };

        var allReceitas = new List<Receita>();

        foreach (var orgao in orgaos)
        {
            foreach (var ano in anos)
            {
                // Cada órgão recebe receita mensal de 2-4 origens
                var qtdOrigens = random.Next(2, 5);
                var origensEscolhidas = origensReceita.OrderBy(_ => random.Next()).Take(qtdOrigens).ToArray();

                for (int mes = 1; mes <= 12; mes++)
                {
                    // Não gerar meses futuros (projeto é até 2025)
                    if (ano == 2025 && mes > 6) break;

                    foreach (var origem in origensEscolhidas)
                    {
                        // Receita base proporcional ao orçamento do órgão
                        var baseReceita = orgao.OrcamentoAtual / 12 / qtdOrigens;
                        // Variação de ±20%
                        var variacao = 1.0m + (decimal)(random.NextDouble() * 0.4 - 0.2);
                        var valor = Math.Round(baseReceita * variacao, 2);

                        allReceitas.Add(new Receita
                        {
                            Valor = valor,
                            Mes = mes,
                            Ano = ano,
                            Origem = origem,
                            OrgaoGovernoId = orgao.Id
                        });
                    }
                }
            }
        }

        context.Receitas.AddRange(allReceitas);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} receitas inseridas.", allReceitas.Count);

        // ──────────────────────────────────────────────
        // 8. Orçamentos (1 por órgão por ano)
        // ──────────────────────────────────────────────
        var allOrcamentos = new List<Orcamento>();

        foreach (var orgao in orgaos)
        {
            foreach (var ano in anos)
            {
                var dotacaoInicial = orgao.OrcamentoAtual * (1.0m + (decimal)(random.NextDouble() * 0.1 - 0.05));
                var suplementacao = dotacaoInicial * (decimal)(random.NextDouble() * 0.15);

                allOrcamentos.Add(new Orcamento
                {
                    Ano = ano,
                    DotacaoInicial = Math.Round(dotacaoInicial, 2),
                    DotacaoAtualizada = Math.Round(dotacaoInicial + suplementacao, 2),
                    OrgaoGovernoId = orgao.Id
                });
            }
        }

        context.Orcamentos.AddRange(allOrcamentos);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} orçamentos inseridos.", allOrcamentos.Count);

        logger.LogInformation("✅ Seed do banco de dados concluído com sucesso!");
    }
}
