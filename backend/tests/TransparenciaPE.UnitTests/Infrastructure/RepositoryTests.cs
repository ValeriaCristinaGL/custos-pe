using FluentAssertions;
using TransparenciaPE.Domain.Entities;
using TransparenciaPE.Infrastructure.Repositories;

namespace TransparenciaPE.UnitTests.Infrastructure;

public class RepositoryTests
{
    [Fact]
    public async Task AddAsync_PersistsEntity_WhenSaved()
    {
        using var context = InMemoryDbContextFactory.Create();
        var repository = new Repository<OrgaoGoverno>(context);
        var orgao = CreateOrgao("001", "Secretaria de Educacao");

        await repository.AddAsync(orgao);
        await context.SaveChangesAsync();

        context.OrgaosGoverno.Should().ContainSingle(o => o.Id == orgao.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
    {
        using var context = InMemoryDbContextFactory.Create();
        var repository = new Repository<OrgaoGoverno>(context);
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        var byId = await repository.GetByIdAsync(orgao.Id);

        byId.Should().NotBeNull();
        byId!.Id.Should().Be(orgao.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEntities_WhenEntitiesExist()
    {
        using var context = InMemoryDbContextFactory.Create();
        var repository = new Repository<OrgaoGoverno>(context);
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        var all = await repository.GetAllAsync();

        all.Should().ContainSingle(o => o.Id == orgao.Id);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingEntities_WhenPredicateMatches()
    {
        using var context = InMemoryDbContextFactory.Create();
        var repository = new Repository<OrgaoGoverno>(context);
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        var found = await repository.FindAsync(o => o.Codigo == "001");

        found.Should().ContainSingle(o => o.Id == orgao.Id);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenPredicateMatches()
    {
        using var context = InMemoryDbContextFactory.Create();
        var repository = new Repository<OrgaoGoverno>(context);
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        var exists = await repository.ExistsAsync(o => o.Nome.Contains("Educacao"));

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Update_PersistsChanges_WhenSaved()
    {
        using var context = InMemoryDbContextFactory.Create();
        var repository = new Repository<OrgaoGoverno>(context);
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        orgao.Nome = "Secretaria Estadual de Educacao";
        repository.Update(orgao);
        await context.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(orgao.Id);
        updated!.Nome.Should().Be("Secretaria Estadual de Educacao");
    }

    [Fact]
    public async Task Remove_DeletesEntity_WhenSaved()
    {
        using var context = InMemoryDbContextFactory.Create();
        var repository = new Repository<OrgaoGoverno>(context);
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.SaveChangesAsync();

        repository.Remove(orgao);
        await context.SaveChangesAsync();

        (await repository.ExistsAsync(o => o.Id == orgao.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task GetByNumeroAsync_ReturnsEmpenhoWithOrgao_WhenEmpenhoExists()
    {
        using var context = InMemoryDbContextFactory.Create();
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        var empenho = CreateEmpenho("EMP-001", 2025, orgao);
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.Empenhos.AddAsync(empenho);
        await context.SaveChangesAsync();
        var repository = new EmpenhoRepository(context);

        var result = await repository.GetByNumeroAsync("EMP-001", 2025);

        result.Should().NotBeNull();
        result!.NumeroEmpenho.Should().Be("EMP-001");
        result.OrgaoGoverno.Nome.Should().Be("Secretaria de Educacao");
    }

    [Fact]
    public async Task GetByAnoAsync_ReturnsOnlyMatchingEmpenhos_WhenYearIsProvided()
    {
        using var context = InMemoryDbContextFactory.Create();
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.Empenhos.AddRangeAsync(
            CreateEmpenho("EMP-001", 2025, orgao),
            CreateEmpenho("EMP-002", 2024, orgao));
        await context.SaveChangesAsync();
        var repository = new EmpenhoRepository(context);

        var result = await repository.GetByAnoAsync(2025);

        result.Should().ContainSingle();
        result.Single().NumeroEmpenho.Should().Be("EMP-001");
    }

    [Fact]
    public async Task GetByOrgaoAsync_ReturnsOnlyMatchingEmpenhos_WhenOrgaoIsProvided()
    {
        using var context = InMemoryDbContextFactory.Create();
        var educacao = CreateOrgao("001", "Secretaria de Educacao");
        var saude = CreateOrgao("002", "Secretaria de Saude");
        await context.OrgaosGoverno.AddRangeAsync(educacao, saude);
        await context.Empenhos.AddRangeAsync(
            CreateEmpenho("EMP-001", 2025, educacao),
            CreateEmpenho("EMP-002", 2025, saude));
        await context.SaveChangesAsync();
        var repository = new EmpenhoRepository(context);

        var result = await repository.GetByOrgaoAsync(educacao.Id);

        result.Should().ContainSingle();
        result.Single().OrgaoGovernoId.Should().Be(educacao.Id);
    }

    [Fact]
    public async Task GetByNumeroAsync_ReturnsContratoWithOrgao_WhenContratoExists()
    {
        using var context = InMemoryDbContextFactory.Create();
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        var contrato = CreateContrato("CT-001", "11222333000181", orgao);
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.Contratos.AddAsync(contrato);
        await context.SaveChangesAsync();
        var repository = new ContratoRepository(context);

        var result = await repository.GetByNumeroAsync("CT-001");

        result.Should().NotBeNull();
        result!.Fornecedor.Should().Be("Empresa A");
        result.OrgaoGoverno.Codigo.Should().Be("001");
    }

    [Fact]
    public async Task SearchByCnpjAsync_ReturnsMatchingContratos_WhenCnpjExists()
    {
        using var context = InMemoryDbContextFactory.Create();
        var orgao = CreateOrgao("001", "Secretaria de Educacao");
        await context.OrgaosGoverno.AddAsync(orgao);
        await context.Contratos.AddRangeAsync(
            CreateContrato("CT-001", "11222333000181", orgao),
            CreateContrato("CT-002", "22333444000155", orgao));
        await context.SaveChangesAsync();
        var repository = new ContratoRepository(context);

        var result = await repository.SearchByCnpjAsync("11222333000181");

        result.Should().ContainSingle();
        result.Single().NumeroContrato.Should().Be("CT-001");
    }

    private static OrgaoGoverno CreateOrgao(string codigo, string nome)
    {
        return new OrgaoGoverno
        {
            Codigo = codigo,
            Nome = nome,
            Sigla = codigo,
            Tipo = "Secretaria"
        };
    }

    private static Empenho CreateEmpenho(string numero, int ano, OrgaoGoverno orgao)
    {
        return new Empenho
        {
            NumeroEmpenho = numero,
            Ano = ano,
            OrgaoGoverno = orgao,
            OrgaoGovernoId = orgao.Id,
            Credor = "Empresa A",
            CnpjCredor = "11222333000181",
            Valor = 100m,
            DataEmpenho = new DateTime(ano, 1, 1),
            Descricao = "Material de consumo",
            ClassificacaoMcasp = "Custeio"
        };
    }

    private static Contrato CreateContrato(string numero, string cnpj, OrgaoGoverno orgao)
    {
        return new Contrato
        {
            NumeroContrato = numero,
            OrgaoGoverno = orgao,
            OrgaoGovernoId = orgao.Id,
            Fornecedor = "Empresa A",
            CnpjFornecedor = cnpj,
            ValorContrato = 100m,
            DataInicio = new DateTime(2025, 1, 1),
            Objeto = "Fornecimento de materiais"
        };
    }
}
