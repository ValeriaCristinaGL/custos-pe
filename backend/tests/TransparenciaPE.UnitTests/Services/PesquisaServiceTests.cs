using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using TransparenciaPE.Application.DTOs;
using TransparenciaPE.Application.Services;
using TransparenciaPE.Domain.Interfaces;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.UnitTests.Services;

public class PesquisaServiceTests
{
    private readonly Mock<IEmpenhoRepository> _mockEmpenhoRepo;
    private readonly Mock<IContratoRepository> _mockContratoRepo;
    private readonly Mock<ILogger<PesquisaService>> _mockLogger;
    private readonly PesquisaService _sut;

    public PesquisaServiceTests()
    {
        _mockEmpenhoRepo = new Mock<IEmpenhoRepository>();
        _mockContratoRepo = new Mock<IContratoRepository>();
        _mockLogger = new Mock<ILogger<PesquisaService>>();
        _sut = new PesquisaService(
            _mockEmpenhoRepo.Object,
            _mockContratoRepo.Object,
            _mockLogger.Object);
    }

    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    public async Task PesquisaGlobalAsync_SearchesContratosByCnpj_WhenTermIsCnpj(string termo)
    {
        // Arrange
        var cnpjLimpo = "11222333000181";
        _mockContratoRepo.Setup(r => r.SearchByCnpjAsync(cnpjLimpo)).ReturnsAsync(new List<Contrato>());
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Empenho, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Empenho>());

        // Act
        var result = await _sut.PesquisaGlobalAsync(termo);

        // Assert
        Assert.NotNull(result);
        _mockContratoRepo.Verify(r => r.SearchByCnpjAsync(cnpjLimpo), Times.Once);
        _mockContratoRepo.Verify(r => r.SearchByFornecedorAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PesquisaGlobalAsync_SearchesContratosByFornecedor_WhenTermIsText()
    {
        // Arrange
        var termo = "Empresa ABC";
        _mockContratoRepo.Setup(r => r.SearchByFornecedorAsync(termo)).ReturnsAsync(new List<Contrato>());
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Empenho, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Empenho>());

        // Act
        var result = await _sut.PesquisaGlobalAsync(termo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(termo, result.TermoBuscado);
        _mockContratoRepo.Verify(r => r.SearchByFornecedorAsync(termo), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task PesquisaGlobalAsync_ThrowsArgumentException_WhenTermIsEmpty(string? termo)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.PesquisaGlobalAsync(termo!));
    }

    [Fact]
    public async Task ExportarCsvAsync_ReturnsCsvWithEmpenhoData_WhenDataExists()
    {
        // Arrange
        var empenhos = new List<Empenho>
        {
            new()
            {
                NumeroEmpenho = "EMP-001",
                Credor = "Empresa A",
                CnpjCredor = "11222333000181",
                Valor = 10_000m,
                DataEmpenho = new DateTime(2025, 1, 1),
                Descricao = "Compra de materiais",
                OrgaoGoverno = new OrgaoGoverno { Nome = "Sec. Educação" }
            }
        };
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Empenho, bool>>>()))
            .ReturnsAsync(empenhos);

        // Act
        var result = await _sut.ExportarCsvAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        var csv = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("EMP-001", csv);
        Assert.Contains("Empresa A", csv);
    }

    [Fact]
    public async Task ExportarCsvAsync_ReturnsHeaderOnly_WhenNoDataExists()
    {
        // Arrange
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Empenho, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Empenho>());

        // Act
        var result = await _sut.ExportarCsvAsync();

        // Assert
        var csv = System.Text.Encoding.UTF8.GetString(result);
        csv.Should().Contain("NumeroEmpenho");
        csv.Split('\n', StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(1); // apenas o header
    }

    [Fact]
    public async Task ExportarCsvAsync_UsesYearFilter_WhenYearIsProvided()
    {
        // Arrange
        Expression<Func<Empenho, bool>>? capturedFilter = null;
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Empenho, bool>>>()))
            .Callback<Expression<Func<Empenho, bool>>>(filter => capturedFilter = filter)
            .ReturnsAsync(Enumerable.Empty<Empenho>());

        // Act
        await _sut.ExportarCsvAsync(ano: 2025);

        // Assert
        capturedFilter.Should().NotBeNull();
        var filter = capturedFilter!.Compile();
        filter(new Empenho { Ano = 2025 }).Should().BeTrue();
        filter(new Empenho { Ano = 2024 }).Should().BeFalse();
    }
}
