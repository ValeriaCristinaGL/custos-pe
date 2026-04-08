using Moq;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Services;
using TransparenciaPE.Domain.Interfaces;

namespace TransparenciaPE.UnitTests.Services;

public class DashboardServiceTests
{
    private readonly Mock<IDashboardQueryService> _mockQueryService;
    private readonly Mock<ILogger<DashboardService>> _mockLogger;
    private readonly DashboardService _sut;

    public DashboardServiceTests()
    {
        _mockQueryService = new Mock<IDashboardQueryService>();
        _mockLogger = new Mock<ILogger<DashboardService>>();
        _sut = new DashboardService(_mockQueryService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetResumoAsync_ShouldReturnKPIs_WhenDataExists()
    {
        // Arrange
        var queryResult = new DashboardResumoResult
        {
            TotalEmpenhado = 1_000_000m,
            TotalLiquidado = 800_000m,
            TotalPago = 600_000m,
            TotalEmpenhos = 150,
            TotalContratos = 30
        };
        _mockQueryService.Setup(q => q.GetResumoAsync(null)).ReturnsAsync(queryResult);

        // Act
        var result = await _sut.GetResumoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1_000_000m, result.TotalEmpenhado);
        Assert.Equal(800_000m, result.TotalLiquidado);
        Assert.Equal(600_000m, result.TotalPago);
        Assert.Equal(60m, result.PercentualExecutado); // 600k / 1M * 100
        Assert.Equal(150, result.TotalEmpenhos);
        _mockQueryService.Verify(q => q.GetResumoAsync(null), Times.Once);
    }

    [Fact]
    public async Task GetResumoAsync_ShouldReturnZeroPercent_WhenNoEmpenhos()
    {
        // Arrange
        var queryResult = new DashboardResumoResult
        {
            TotalEmpenhado = 0,
            TotalLiquidado = 0,
            TotalPago = 0,
            TotalEmpenhos = 0,
            TotalContratos = 0
        };
        _mockQueryService.Setup(q => q.GetResumoAsync(null)).ReturnsAsync(queryResult);

        // Act
        var result = await _sut.GetResumoAsync();

        // Assert
        Assert.Equal(0m, result.PercentualExecutado);
    }

    [Fact]
    public async Task GetResumoAsync_ShouldFilterByYear_WhenYearProvided()
    {
        // Arrange
        var queryResult = new DashboardResumoResult { TotalEmpenhado = 500_000m };
        _mockQueryService.Setup(q => q.GetResumoAsync(2025)).ReturnsAsync(queryResult);

        // Act
        var result = await _sut.GetResumoAsync(2025);

        // Assert
        Assert.Equal(500_000m, result.TotalEmpenhado);
        _mockQueryService.Verify(q => q.GetResumoAsync(2025), Times.Once);
    }

    [Fact]
    public async Task GetComparativoOrgaosAsync_ShouldReturnOrgaoList()
    {
        // Arrange
        var queryResults = new List<ComparativoOrgaoResult>
        {
            new() { CodigoOrgao = "001", NomeOrgao = "Secretaria de Educação", TotalEmpenhado = 500_000m },
            new() { CodigoOrgao = "002", NomeOrgao = "Secretaria de Saúde", TotalEmpenhado = 300_000m }
        };
        _mockQueryService.Setup(q => q.GetComparativoOrgaosAsync(2025)).ReturnsAsync(queryResults);

        // Act
        var result = await _sut.GetComparativoOrgaosAsync(2025);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2025, result.Ano);
        Assert.Equal(2, result.Orgaos.Count());
    }

    [Fact]
    public async Task GetDrillDownAsync_ShouldReturnHierarchicalData()
    {
        // Arrange
        var queryResults = new List<DrillDownResult>
        {
            new() { ClassificacaoMcasp = "3.3.90.30", Descricao = "Material de Consumo", TotalEmpenhado = 100_000m, QuantidadeEmpenhos = 20 }
        };
        _mockQueryService.Setup(q => q.GetDrillDownAsync("001", null)).ReturnsAsync(queryResults);

        // Act
        var result = await _sut.GetDrillDownAsync("001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("001", result.CodigoOrgao);
        Assert.Single(result.Itens);
    }
}
