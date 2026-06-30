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
    public async Task GetResumo_ReturnsOkResult_WhenServiceReturnsResumo()
    {
        // Arrange
        _mockService.Setup(s => s.GetResumoAsync(null))
            .ReturnsAsync(new DashboardResumoDto { TotalEmpenhado = 1_000_000m });

        // Act
        var result = await _sut.GetResumo(null);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(200, okResult!.StatusCode);
    }

    [Fact]
    public async Task GetResumo_ReturnsDashboardResumoDto_WhenServiceReturnsResumo()
    {
        // Arrange
        _mockService.Setup(s => s.GetResumoAsync(null))
            .ReturnsAsync(new DashboardResumoDto());

        // Act
        var result = await _sut.GetResumo(null);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsType<DashboardResumoDto>(okResult!.Value);
    }

    [Fact]
    public async Task GetResumo_ReturnsTotalEmpenhado_WhenServiceReturnsResumo()
    {
        // Arrange
        _mockService.Setup(s => s.GetResumoAsync(null))
            .ReturnsAsync(new DashboardResumoDto { TotalEmpenhado = 1_000_000m });

        // Act
        var result = await _sut.GetResumo(null);

        // Assert
        var returned = (result.Result as OkObjectResult)!.Value as DashboardResumoDto;
        Assert.Equal(1_000_000m, returned!.TotalEmpenhado);
    }

    [Fact]
    public async Task GetResumo_InvokesServiceWithNullYear_WhenAnoIsNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetResumoAsync(null))
            .ReturnsAsync(new DashboardResumoDto());

        // Act
        await _sut.GetResumo(null);

        // Assert
        _mockService.Verify(s => s.GetResumoAsync(null), Times.Once);
    }

    [Fact]
    public async Task GetResumo_InvokesServiceWithProvidedYear_WhenAnoIsProvided()
    {
        // Arrange
        _mockService.Setup(s => s.GetResumoAsync(2025))
            .ReturnsAsync(new DashboardResumoDto());

        // Act
        await _sut.GetResumo(2025);

        // Assert
        _mockService.Verify(s => s.GetResumoAsync(2025), Times.Once);
    }

    [Fact]
    public async Task GetComparativo_ReturnsOkResult_WhenServiceReturnsComparativo()
    {
        // Arrange
        _mockService.Setup(s => s.GetComparativoOrgaosAsync(2025))
            .ReturnsAsync(new ComparativoOrgaosDto { Ano = 2025 });

        // Act
        var result = await _sut.GetComparativo(2025);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(200, okResult!.StatusCode);
    }

    [Fact]
    public async Task GetComparativo_ReturnsComparativoOrgaosDto_WhenServiceReturnsComparativo()
    {
        // Arrange
        _mockService.Setup(s => s.GetComparativoOrgaosAsync(2025))
            .ReturnsAsync(new ComparativoOrgaosDto { Ano = 2025 });

        // Act
        var result = await _sut.GetComparativo(2025);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsType<ComparativoOrgaosDto>(okResult!.Value);
    }

    [Fact]
    public async Task GetComparativo_ReturnsAno_WhenServiceReturnsComparativo()
    {
        // Arrange
        _mockService.Setup(s => s.GetComparativoOrgaosAsync(2025))
            .ReturnsAsync(new ComparativoOrgaosDto { Ano = 2025 });

        // Act
        var result = await _sut.GetComparativo(2025);

        // Assert
        var returned = (result.Result as OkObjectResult)!.Value as ComparativoOrgaosDto;
        Assert.Equal(2025, returned!.Ano);
    }

    [Fact]
    public async Task GetEvolucao_ReturnsOkResult_WhenServiceReturnsDrillDown()
    {
        // Arrange
        _mockService.Setup(s => s.GetDrillDownAsync("001", null))
            .ReturnsAsync(new DrillDownDto { CodigoOrgao = "001" });

        // Act
        var result = await _sut.GetEvolucao("001", null);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(200, okResult!.StatusCode);
    }

    [Fact]
    public async Task GetEvolucao_ReturnsDrillDownDto_WhenServiceReturnsDrillDown()
    {
        // Arrange
        _mockService.Setup(s => s.GetDrillDownAsync("001", null))
            .ReturnsAsync(new DrillDownDto { CodigoOrgao = "001" });

        // Act
        var result = await _sut.GetEvolucao("001", null);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsType<DrillDownDto>(okResult!.Value);
    }

    [Fact]
    public async Task GetEvolucao_ReturnsCodigoOrgao_WhenServiceReturnsDrillDown()
    {
        // Arrange
        _mockService.Setup(s => s.GetDrillDownAsync("001", null))
            .ReturnsAsync(new DrillDownDto { CodigoOrgao = "001" });

        // Act
        var result = await _sut.GetEvolucao("001", null);

        // Assert
        var returned = (result.Result as OkObjectResult)!.Value as DrillDownDto;
        Assert.Equal("001", returned!.CodigoOrgao);
    }

    [Fact]
    public async Task GetEvolucao_InvokesServiceWithNullYear_WhenAnoIsNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetDrillDownAsync("001", null))
            .ReturnsAsync(new DrillDownDto { CodigoOrgao = "001" });

        // Act
        await _sut.GetEvolucao("001", null);

        // Assert
        _mockService.Verify(s => s.GetDrillDownAsync("001", null), Times.Once);
    }

    [Fact]
    public async Task GetEvolucao_InvokesServiceWithProvidedParameters_WhenAnoIsProvided()
    {
        // Arrange
        _mockService.Setup(s => s.GetDrillDownAsync("002", 2025))
            .ReturnsAsync(new DrillDownDto { CodigoOrgao = "002" });

        // Act
        await _sut.GetEvolucao("002", 2025);

        // Assert
        _mockService.Verify(s => s.GetDrillDownAsync("002", 2025), Times.Once);
    }
}
