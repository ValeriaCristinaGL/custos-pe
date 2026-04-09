using Moq;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.Services;
using TransparenciaPE.Domain.Interfaces;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Services;

public class DataSyncServiceTests
{
    private readonly Mock<IPEDataClient> _mockDataClient;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEmpenhoRepository> _mockEmpenhoRepo;
    private readonly Mock<IContratoRepository> _mockContratoRepo;
    private readonly Mock<IRepository<OrgaoGoverno>> _mockOrgaoRepo;
    private readonly Mock<ILogger<DataSyncService>> _mockLogger;
    private readonly DataSyncService _sut;

    public DataSyncServiceTests()
    {
        _mockDataClient = new Mock<IPEDataClient>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEmpenhoRepo = new Mock<IEmpenhoRepository>();
        _mockContratoRepo = new Mock<IContratoRepository>();
        _mockOrgaoRepo = new Mock<IRepository<OrgaoGoverno>>();
        _mockLogger = new Mock<ILogger<DataSyncService>>();

        _mockUnitOfWork.Setup(u => u.Empenhos).Returns(_mockEmpenhoRepo.Object);
        _mockUnitOfWork.Setup(u => u.Contratos).Returns(_mockContratoRepo.Object);

        _sut = new DataSyncService(
            _mockDataClient.Object,
            _mockUnitOfWork.Object,
            _mockOrgaoRepo.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task SyncEmpenhosAsync_ShouldInsertNewEmpenhos()
    {
        // Arrange
        var externalData = new List<ExternalEmpenhoData>
        {
            new()
            {
                NumeroEmpenho = "EMP-001",
                Ano = 2025,
                NomeOrgao = "Secretaria Educação",
                CodigoOrgao = "001",
                SiglaOrgao = "SEDUC",
                Credor = "Empresa A",
                CnpjCredor = "11.222.333/0001-81",
                Valor = 50_000m,
                DataEmpenho = new DateTime(2025, 3, 15),
                Descricao = "Material escolar",
                NaturezaDespesa = "3.3.90.30"
            }
        };
        _mockDataClient.Setup(c => c.GetEmpenhosAsync(2025)).ReturnsAsync(externalData);
        _mockEmpenhoRepo.Setup(r => r.GetByNumeroAsync("EMP-001", 2025)).ReturnsAsync((Empenho?)null);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<OrgaoGoverno>());

        // Act
        var count = await _sut.SyncEmpenhosAsync(2025);

        // Assert
        Assert.Equal(1, count);
        _mockEmpenhoRepo.Verify(r => r.AddAsync(It.Is<Empenho>(e =>
            e.NumeroEmpenho == "EMP-001" &&
            e.CnpjCredor == "11222333000181")), Times.Once); // CNPJ sanitizado!
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SyncEmpenhosAsync_ShouldUpdateExistingEmpenho()
    {
        // Arrange
        var existingEmpenho = new Empenho
        {
            Id = Guid.NewGuid(),
            NumeroEmpenho = "EMP-001",
            Ano = 2025,
            Valor = 30_000m
        };
        var externalData = new List<ExternalEmpenhoData>
        {
            new()
            {
                NumeroEmpenho = "EMP-001",
                Ano = 2025,
                CodigoOrgao = "001",
                NomeOrgao = "Secretaria Educação",
                SiglaOrgao = "SEDUC",
                Credor = "Empresa A",
                CnpjCredor = "11222333000181",
                Valor = 50_000m, // valor atualizado
                DataEmpenho = new DateTime(2025, 3, 15),
                NaturezaDespesa = "3.3.90.30"
            }
        };
        _mockDataClient.Setup(c => c.GetEmpenhosAsync(2025)).ReturnsAsync(externalData);
        _mockEmpenhoRepo.Setup(r => r.GetByNumeroAsync("EMP-001", 2025)).ReturnsAsync(existingEmpenho);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(new List<OrgaoGoverno> { new() { Codigo = "001" } });

        // Act
        var count = await _sut.SyncEmpenhosAsync(2025);

        // Assert
        Assert.Equal(1, count);
        Assert.Equal(50_000m, existingEmpenho.Valor); // Upsert: valor atualizado
        _mockEmpenhoRepo.Verify(r => r.AddAsync(It.IsAny<Empenho>()), Times.Never); // Não insere novo
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SyncEmpenhosAsync_ShouldSanitizeCnpj()
    {
        // Arrange
        var externalData = new List<ExternalEmpenhoData>
        {
            new()
            {
                NumeroEmpenho = "EMP-002",
                Ano = 2025,
                CodigoOrgao = "001",
                NomeOrgao = "Sec Saúde",
                SiglaOrgao = "SES",
                CnpjCredor = "11.222.333/0001-81", // Com pontuação
                Valor = 10_000m,
                DataEmpenho = DateTime.UtcNow,
                NaturezaDespesa = "3.3.90.39"
            }
        };
        _mockDataClient.Setup(c => c.GetEmpenhosAsync(2025)).ReturnsAsync(externalData);
        _mockEmpenhoRepo.Setup(r => r.GetByNumeroAsync("EMP-002", 2025)).ReturnsAsync((Empenho?)null);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<OrgaoGoverno>());

        // Act
        await _sut.SyncEmpenhosAsync(2025);

        // Assert
        _mockEmpenhoRepo.Verify(r => r.AddAsync(It.Is<Empenho>(e =>
            e.CnpjCredor == "11222333000181")), Times.Once);
    }

    [Fact]
    public async Task SyncAllAsync_ShouldReturnCombinedResult()
    {
        // Arrange
        _mockDataClient.Setup(c => c.GetEmpenhosAsync(2025))
            .ReturnsAsync(new List<ExternalEmpenhoData>());
        _mockDataClient.Setup(c => c.GetContratosAsync(2025))
            .ReturnsAsync(new List<ExternalContratoData>());

        // Act
        var result = await _sut.SyncAllAsync(2025);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.SyncedAt <= DateTime.UtcNow);
    }
}
