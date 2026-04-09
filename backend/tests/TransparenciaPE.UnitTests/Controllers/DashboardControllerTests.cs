using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransparenciaPE.API.Controllers;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Interfaces;

namespace TransparenciaPE.UnitTests.Controllers;

public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _mockService;
    private readonly Mock<ILogger<DashboardController>> _mockLogger;
    private readonly DashboardController _sut;

    public DashboardControllerTests()
    {
        _mockService = new Mock<IDashboardService>();
        _mockLogger = new Mock<ILogger<DashboardController>>();
        _sut = new DashboardController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetResumo_ShouldReturn200_WithKPIs()
    {
        // Arrange
        var dto = new DashboardResumoDto
        {
            TotalEmpenhado = 1_000_000m,
            TotalLiquidado = 800_000m,
            TotalPago = 600_000m,
            PercentualExecutado = 60m
        };
        _mockService.Setup(s => s.GetResumoAsync(null)).ReturnsAsync(dto);

        // Act
        var result = await _sut.GetResumo(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<DashboardResumoDto>(okResult.Value);
        Assert.Equal(1_000_000m, returned.TotalEmpenhado);
    }

    [Fact]
    public async Task GetComparativoOrgaos_ShouldReturn200_WithOrgaoList()
    {
        // Arrange
        var dto = new ComparativoOrgaosDto
        {
            Ano = 2025,
            Orgaos = new List<OrgaoComparativoItem>
            {
                new() { NomeOrgao = "Sec. Educação", TotalEmpenhado = 500_000m }
            }
        };
        _mockService.Setup(s => s.GetComparativoOrgaosAsync(2025)).ReturnsAsync(dto);

        // Act
        var result = await _sut.GetComparativo(2025);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<ComparativoOrgaosDto>(okResult.Value);
        Assert.Equal(2025, returned.Ano);
    }

    [Fact]
    public async Task GetDrillDown_ShouldReturn200_WithHierarchicalData()
    {
        // Arrange
        var dto = new DrillDownDto
        {
            CodigoOrgao = "001",
            Itens = new List<DrillDownItem>
            {
                new() { ClassificacaoMcasp = "3.3.90.30", TotalEmpenhado = 100_000m }
            }
        };
        _mockService.Setup(s => s.GetDrillDownAsync("001", null)).ReturnsAsync(dto);

        // Act
        var result = await _sut.GetEvolucao("001", null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<DrillDownDto>(okResult.Value);
        Assert.Equal("001", returned.CodigoOrgao);
    }
}
