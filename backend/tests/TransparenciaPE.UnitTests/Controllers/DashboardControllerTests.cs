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

    // ─── GetResumo ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetResumo_Returns200StatusCode()
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
    public async Task GetResumo_ReturnsDashboardResumoDtoAsBody()
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
    public async Task GetResumo_ReturnsTotalEmpenhadoFromService()
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

    [Theory]
    [InlineData(null)]
    [InlineData(2025)]
    public async Task GetResumo_InvokesServiceWithProvidedYear(int? ano)
    {
        // Arrange
        _mockService.Setup(s => s.GetResumoAsync(ano))
            .ReturnsAsync(new DashboardResumoDto());

        // Act
        await _sut.GetResumo(ano);

        // Assert
        _mockService.Verify(s => s.GetResumoAsync(ano), Times.Once);
    }

    // ─── GetComparativo ───────────────────────────────────────────────────

    [Fact]
    public async Task GetComparativo_Returns200StatusCode()
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
    public async Task GetComparativo_ReturnsComparativoOrgaosDtoAsBody()
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
    public async Task GetComparativo_ReturnsAnoFromService()
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

    // ─── GetEvolucao ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetEvolucao_Returns200StatusCode()
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
    public async Task GetEvolucao_ReturnsDrillDownDtoAsBody()
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
    public async Task GetEvolucao_ReturnsCodigoOrgaoFromService()
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

    [Theory]
    [InlineData("001", null)]
    [InlineData("002", 2025)]
    public async Task GetEvolucao_InvokesServiceWithProvidedParameters(string codigoOrgao, int? ano)
    {
        // Arrange
        _mockService.Setup(s => s.GetDrillDownAsync(codigoOrgao, ano))
            .ReturnsAsync(new DrillDownDto { CodigoOrgao = codigoOrgao });

        // Act
        await _sut.GetEvolucao(codigoOrgao, ano);

        // Assert
        _mockService.Verify(s => s.GetDrillDownAsync(codigoOrgao, ano), Times.Once);
    }
}

