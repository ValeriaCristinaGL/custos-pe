using FluentAssertions;
using TransparenciaPE.Domain.Entities;
using TransparenciaPE.Infrastructure.Repositories;
using InfrastructureUnitOfWork = TransparenciaPE.Infrastructure.UnitOfWork.UnitOfWork;

namespace TransparenciaPE.UnitTests.Infrastructure;

public class UnitOfWorkTests
{
    [Fact]
    public void UnitOfWork_ExposesConfiguredRepositories_WhenCreated()
    {
        using var context = InMemoryDbContextFactory.Create();
        var empenhoRepository = new EmpenhoRepository(context);
        var contratoRepository = new ContratoRepository(context);

        using var unitOfWork = new InfrastructureUnitOfWork(context, empenhoRepository, contratoRepository);

        unitOfWork.Empenhos.Should().BeSameAs(empenhoRepository);
        unitOfWork.Contratos.Should().BeSameAs(contratoRepository);
    }

    [Fact]
    public async Task CommitAsync_PersistsPendingChanges_WhenRepositoryAddsEntity()
    {
        using var context = InMemoryDbContextFactory.Create();
        var empenhoRepository = new EmpenhoRepository(context);
        var contratoRepository = new ContratoRepository(context);
        using var unitOfWork = new InfrastructureUnitOfWork(context, empenhoRepository, contratoRepository);
        var orgao = new OrgaoGoverno
        {
            Codigo = "001",
            Nome = "Secretaria de Educacao",
            Sigla = "SEDUC",
            Tipo = "Secretaria"
        };

        await context.OrgaosGoverno.AddAsync(orgao);
        var changes = await unitOfWork.CommitAsync();

        changes.Should().Be(1);
        context.OrgaosGoverno.Should().ContainSingle(o => o.Id == orgao.Id);
    }

    [Fact]
    public void Dispose_DisposesDbContext_WhenCalled()
    {
        var context = InMemoryDbContextFactory.Create();
        var empenhoRepository = new EmpenhoRepository(context);
        var contratoRepository = new ContratoRepository(context);
        var unitOfWork = new InfrastructureUnitOfWork(context, empenhoRepository, contratoRepository);

        unitOfWork.Dispose();

        var act = () => context.OrgaosGoverno.Count();
        act.Should().Throw<ObjectDisposedException>();
    }
}
