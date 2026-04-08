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

    [Fact]
    public async Task PesquisaGlobal_ShouldReturn200_WithResults()
    {
        // Arrange
        var dto = new PesquisaResultDto
        {
            TermoBuscado = "Empresa A",
            TotalResultados = 3,
            Resultados = new List<PesquisaItem>
            {
                new() { Tipo = "Contrato", Numero = "CT-001", Fornecedor = "Empresa A" }
            }
        };
        _mockService.Setup(s => s.PesquisaGlobalAsync("Empresa A")).ReturnsAsync(dto);

        // Act
        var result = await _sut.PesquisaGlobal("Empresa A");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<PesquisaResultDto>(okResult.Value);
        Assert.Equal("Empresa A", returned.TermoBuscado);
    }

    [Fact]
    public async Task PesquisaGlobal_ShouldReturn400_WhenTermoIsEmpty()
    {
        // Arrange
        _mockService.Setup(s => s.PesquisaGlobalAsync(""))
            .ThrowsAsync(new ArgumentException("O termo de busca é obrigatório."));

        // Act
        var result = await _sut.PesquisaGlobal("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ExportarCsv_ShouldReturn200_WithFileResult()
    {
        // Arrange
        var csvBytes = System.Text.Encoding.UTF8.GetBytes("NumeroEmpenho;Orgao\nEMP-001;Sec Edu");
        _mockService.Setup(s => s.ExportarCsvAsync(null, null)).ReturnsAsync(csvBytes);

        // Act
        var result = await _sut.ExportarCsv(null, null);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Contains(".csv", fileResult.FileDownloadName);
    }
}
