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
    public async Task SyncEmpenhosAsync_AddsEmpenho_WhenEmpenhoDoesNotExist()
    {
        // Arrange
        var orgaosBase = new List<OrgaoGoverno>
        {
            new() { Codigo = "001", Nome = "Secretaria Educação", Sigla = "SEDUC", Tipo = "Secretaria" }
        };
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
        _mockOrgaoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orgaosBase);
        _mockDataClient.Setup(c => c.GetEmpenhosByOrgaoAsync(2025, "001")).ReturnsAsync(externalData);
        _mockEmpenhoRepo.Setup(r => r.GetByNumeroAsync("EMP-001", 2025)).ReturnsAsync((Empenho?)null);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(orgaosBase);

        // Act
        var count = await _sut.SyncEmpenhosAsync(2025);

        // Assert
        Assert.Equal(1, count);
        _mockEmpenhoRepo.Verify(r => r.AddAsync(It.Is<Empenho>(e =>
            e.NumeroEmpenho == "EMP-001" &&
            e.CnpjCredor == "11222333000181")), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SyncEmpenhosAsync_UpdatesEmpenho_WhenEmpenhoExists()
    {
        // Arrange
        var orgaosBase = new List<OrgaoGoverno>
        {
            new() { Codigo = "001", Nome = "Secretaria Educação", Sigla = "SEDUC", Tipo = "Secretaria" }
        };
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
                Valor = 50_000m,
                DataEmpenho = new DateTime(2025, 3, 15),
                NaturezaDespesa = "3.3.90.30"
            }
        };
        _mockOrgaoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orgaosBase);
        _mockDataClient.Setup(c => c.GetEmpenhosByOrgaoAsync(2025, "001")).ReturnsAsync(externalData);
        _mockEmpenhoRepo.Setup(r => r.GetByNumeroAsync("EMP-001", 2025)).ReturnsAsync(existingEmpenho);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(orgaosBase);

        // Act
        var count = await _sut.SyncEmpenhosAsync(2025);

        // Assert
        Assert.Equal(1, count);
        Assert.Equal(50_000m, existingEmpenho.Valor);
        _mockEmpenhoRepo.Verify(r => r.AddAsync(It.IsAny<Empenho>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SyncEmpenhosAsync_SanitizesCnpj_WhenAddingEmpenho()
    {
        // Arrange
        var orgaosBase = new List<OrgaoGoverno>
        {
            new() { Codigo = "001", Nome = "Sec Saúde", Sigla = "SES", Tipo = "Secretaria" }
        };
        var externalData = new List<ExternalEmpenhoData>
        {
            new()
            {
                NumeroEmpenho = "EMP-002",
                Ano = 2025,
                CodigoOrgao = "001",
                NomeOrgao = "Sec Saúde",
                SiglaOrgao = "SES",
                CnpjCredor = "11.222.333/0001-81",
                Valor = 10_000m,
                DataEmpenho = DateTime.UtcNow,
                NaturezaDespesa = "3.3.90.39"
            }
        };
        _mockOrgaoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orgaosBase);
        _mockDataClient.Setup(c => c.GetEmpenhosByOrgaoAsync(2025, "001")).ReturnsAsync(externalData);
        _mockEmpenhoRepo.Setup(r => r.GetByNumeroAsync("EMP-002", 2025)).ReturnsAsync((Empenho?)null);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(orgaosBase);

        // Act
        await _sut.SyncEmpenhosAsync(2025);

        // Assert
        _mockEmpenhoRepo.Verify(r => r.AddAsync(It.Is<Empenho>(e =>
            e.CnpjCredor == "11222333000181")), Times.Once);
    }

    [Fact]
    public async Task SyncAllAsync_ReturnsSyncedAt_WhenSyncCompletes()
    {
        // Arrange
        _mockOrgaoRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<OrgaoGoverno> { new() { Codigo = "001", Nome = "Sec", Sigla = "S", Tipo = "Secretaria" } });
        _mockDataClient.Setup(c => c.GetEmpenhosByOrgaoAsync(2025, "001"))
            .ReturnsAsync(new List<ExternalEmpenhoData>());
        _mockDataClient.Setup(c => c.GetContratosAsync(2025))
            .ReturnsAsync(new List<ExternalContratoData>());

        // Act
        var result = await _sut.SyncAllAsync(2025);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.SyncedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task SyncEmpenhosAsync_ReturnsZero_WhenNoOrgaosExist()
    {
        // Arrange
        _mockOrgaoRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<OrgaoGoverno>());

        // Act
        var count = await _sut.SyncEmpenhosAsync(2025);

        // Assert
        Assert.Equal(0, count);
        _mockDataClient.Verify(c => c.GetEmpenhosByOrgaoAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task SyncEmpenhosAsync_ReturnsZero_WhenApiReturnsEmpty()
    {
        // Arrange
        var orgaos = new List<OrgaoGoverno>
        {
            new() { Codigo = "001", Nome = "Sec. Educação", Sigla = "SEDUC", Tipo = "Secretaria" }
        };
        _mockOrgaoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orgaos);
        _mockDataClient.Setup(c => c.GetEmpenhosByOrgaoAsync(2025, "001"))
            .ReturnsAsync(new List<ExternalEmpenhoData>());

        // Act
        var count = await _sut.SyncEmpenhosAsync(2025);

        // Assert
        Assert.Equal(0, count);
        _mockEmpenhoRepo.Verify(r => r.AddAsync(It.IsAny<Empenho>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SyncContratosAsync_AddsContrato_WhenContratoDoesNotExist()
    {
        // Arrange
        var externalData = new List<ExternalContratoData>
        {
            new()
            {
                NumeroContrato = "CT-2025-001",
                CodigoOrgao = "001",
                NomeOrgao = "Sec. Saúde",
                Fornecedor = "Farmácia Central",
                CnpjFornecedor = "22.333.444/0001-55",
                ValorContrato = 200_000m,
                DataInicio = new DateTime(2025, 1, 1),
                Objeto = "Fornecimento de medicamentos"
            }
        };
        _mockDataClient.Setup(c => c.GetContratosAsync(2025)).ReturnsAsync(externalData);
        _mockContratoRepo.Setup(r => r.GetByNumeroAsync("CT-2025-001")).ReturnsAsync((Contrato?)null);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(new List<OrgaoGoverno> { new() { Codigo = "001", Nome = "Sec. Saúde", Sigla = "SES", Tipo = "Secretaria" } });

        // Act
        var count = await _sut.SyncContratosAsync(2025);

        // Assert
        Assert.Equal(1, count);
        _mockContratoRepo.Verify(r => r.AddAsync(It.Is<Contrato>(c =>
            c.NumeroContrato == "CT-2025-001" &&
            c.CnpjFornecedor == "22333444000155")), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SyncAllAsync_ReturnsProcessedCounts_WhenEmpenhosAndContratosExist()
    {
        // Arrange
        var orgaos = new List<OrgaoGoverno>
        {
            new() { Codigo = "001", Nome = "Sec", Sigla = "S", Tipo = "Secretaria" }
        };
        var empenhoData = new List<ExternalEmpenhoData>
        {
            new()
            {
                NumeroEmpenho = "EMP-X", Ano = 2025, CodigoOrgao = "001",
                NomeOrgao = "Sec", SiglaOrgao = "S", Credor = "Empresa A",
                CnpjCredor = "11222333000181", Valor = 10_000m,
                DataEmpenho = DateTime.UtcNow, NaturezaDespesa = "3.3.90.30"
            }
        };
        var contratoData = new List<ExternalContratoData>
        {
            new()
            {
                NumeroContrato = "CT-X", CodigoOrgao = "001", NomeOrgao = "Sec",
                Fornecedor = "Empresa B", CnpjFornecedor = "11222333000181",
                ValorContrato = 50_000m, DataInicio = DateTime.UtcNow, Objeto = "Objeto"
            }
        };

        _mockOrgaoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orgaos);
        _mockOrgaoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrgaoGoverno, bool>>>()))
            .ReturnsAsync(orgaos);
        _mockDataClient.Setup(c => c.GetEmpenhosByOrgaoAsync(2025, "001")).ReturnsAsync(empenhoData);
        _mockDataClient.Setup(c => c.GetContratosAsync(2025)).ReturnsAsync(contratoData);
        _mockEmpenhoRepo.Setup(r => r.GetByNumeroAsync("EMP-X", 2025)).ReturnsAsync((Empenho?)null);
        _mockContratoRepo.Setup(r => r.GetByNumeroAsync("CT-X")).ReturnsAsync((Contrato?)null);

        // Act
        var result = await _sut.SyncAllAsync(2025);

        // Assert
        Assert.Equal(1, result.EmpenhosProcessados);
        Assert.Equal(1, result.ContratosProcessados);
    }
}
