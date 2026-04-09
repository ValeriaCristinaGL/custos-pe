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

    [Fact]
    public async Task PesquisaGlobalAsync_ShouldSearchByCnpj_WhenTermLooksCnpj()
    {
        // Arrange
        var cnpj = "11222333000181";
        var contratos = new List<Contrato>
        {
            new()
            {
                NumeroContrato = "CT-001",
                Fornecedor = "Empresa A",
                CnpjFornecedor = cnpj,
                ValorContrato = 50_000m,
                DataInicio = new DateTime(2025, 1, 1),
                Objeto = "Fornecimento de material",
                OrgaoGoverno = new OrgaoGoverno { Nome = "Secretaria de Educação" }
            }
        };
        _mockContratoRepo.Setup(r => r.SearchByCnpjAsync(cnpj)).ReturnsAsync(contratos);
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Empenho, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Empenho>());

        // Act
        var result = await _sut.PesquisaGlobalAsync(cnpj);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cnpj, result.TermoBuscado);
        Assert.True(result.TotalResultados > 0);
    }

    [Fact]
    public async Task PesquisaGlobalAsync_ShouldSearchByName_WhenTermIsText()
    {
        // Arrange
        var termo = "Empresa ABC";
        _mockContratoRepo.Setup(r => r.SearchByFornecedorAsync(termo)).ReturnsAsync(new List<Contrato>());
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Empenho, bool>>>()))
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
    public async Task PesquisaGlobalAsync_ShouldThrow_WhenTermIsEmpty(string? termo)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.PesquisaGlobalAsync(termo!));
    }

    [Fact]
    public async Task ExportarCsvAsync_ShouldReturnBytes_WhenDataExists()
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
        _mockEmpenhoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Empenho, bool>>>()))
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
}
