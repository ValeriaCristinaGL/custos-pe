using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransparenciaPE.API.Controllers;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Interfaces;

namespace TransparenciaPE.UnitTests.Controllers;

public class PesquisaControllerTests
{
    private readonly Mock<IPesquisaService> _mockService;
    private readonly Mock<ILogger<PesquisaController>> _mockLogger;
    private readonly PesquisaController _sut;

    public PesquisaControllerTests()
    {
        _mockService = new Mock<IPesquisaService>();
        _mockLogger = new Mock<ILogger<PesquisaController>>();
        _sut = new PesquisaController(_mockService.Object, _mockLogger.Object);
    }

    // ─── PesquisaGlobal — cenário de sucesso ─────────────────────────────

    [Fact]
    public async Task PesquisaGlobal_Returns200StatusCode()
    {
        // Arrange
        _mockService.Setup(s => s.PesquisaGlobalAsync("Empresa A"))
            .ReturnsAsync(new PesquisaResultDto { TermoBuscado = "Empresa A" });

        // Act
        var result = await _sut.PesquisaGlobal("Empresa A");

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(200, okResult!.StatusCode);
    }

    [Fact]
    public async Task PesquisaGlobal_ReturnsPesquisaResultDtoAsBody()
    {
        // Arrange
        _mockService.Setup(s => s.PesquisaGlobalAsync("Empresa A"))
            .ReturnsAsync(new PesquisaResultDto { TermoBuscado = "Empresa A" });

        // Act
        var result = await _sut.PesquisaGlobal("Empresa A");

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsType<PesquisaResultDto>(okResult!.Value);
    }

    [Fact]
    public async Task PesquisaGlobal_ReturnsTermoBuscadoFromService()
    {
        // Arrange
        _mockService.Setup(s => s.PesquisaGlobalAsync("Empresa A"))
            .ReturnsAsync(new PesquisaResultDto { TermoBuscado = "Empresa A" });

        // Act
        var result = await _sut.PesquisaGlobal("Empresa A");

        // Assert
        var returned = (result.Result as OkObjectResult)!.Value as PesquisaResultDto;
        Assert.Equal("Empresa A", returned!.TermoBuscado);
    }

    // ─── PesquisaGlobal — cenário de erro ────────────────────────────────

    [Fact]
    public async Task PesquisaGlobal_Returns400StatusCode_WhenTermoIsEmpty()
    {
        // Arrange
        _mockService.Setup(s => s.PesquisaGlobalAsync(""))
            .ThrowsAsync(new ArgumentException("O termo de busca é obrigatório."));

        // Act
        var result = await _sut.PesquisaGlobal("");

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.Equal(400, badRequestResult!.StatusCode);
    }

    [Fact]
    public async Task PesquisaGlobal_ReturnsBadRequestObjectResult_WhenTermoIsEmpty()
    {
        // Arrange
        _mockService.Setup(s => s.PesquisaGlobalAsync(""))
            .ThrowsAsync(new ArgumentException("O termo de busca é obrigatório."));

        // Act
        var result = await _sut.PesquisaGlobal("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // ─── ExportarCsv ─────────────────────────────────────────────────────

    [Fact]
    public async Task ExportarCsv_ReturnsFileContentResult()
    {
        // Arrange
        var csvBytes = System.Text.Encoding.UTF8.GetBytes("NumeroEmpenho;Orgao\nEMP-001;Sec Edu");
        _mockService.Setup(s => s.ExportarCsvAsync(null, null)).ReturnsAsync(csvBytes);

        // Act
        var result = await _sut.ExportarCsv(null, null);

        // Assert
        Assert.IsType<FileContentResult>(result);
    }

    [Fact]
    public async Task ExportarCsv_ReturnsCsvContentType()
    {
        // Arrange
        var csvBytes = System.Text.Encoding.UTF8.GetBytes("NumeroEmpenho;Orgao\nEMP-001;Sec Edu");
        _mockService.Setup(s => s.ExportarCsvAsync(null, null)).ReturnsAsync(csvBytes);

        // Act
        var result = await _sut.ExportarCsv(null, null);

        // Assert
        var fileResult = result as FileContentResult;
        Assert.Equal("text/csv", fileResult!.ContentType);
    }

    [Fact]
    public async Task ExportarCsv_ReturnsFileNameWithCsvExtension()
    {
        // Arrange
        var csvBytes = System.Text.Encoding.UTF8.GetBytes("NumeroEmpenho;Orgao\nEMP-001;Sec Edu");
        _mockService.Setup(s => s.ExportarCsvAsync(null, null)).ReturnsAsync(csvBytes);

        // Act
        var result = await _sut.ExportarCsv(null, null);

        // Assert
        var fileResult = result as FileContentResult;
        Assert.Contains(".csv", fileResult!.FileDownloadName);
    }
}

