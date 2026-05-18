using TransparenciaPE.Application.Helpers;

namespace TransparenciaPE.UnitTests.Helpers;

public class CnpjHelperTests
{
    [Theory]
    [InlineData("11.222.333/0001-81", "11222333000181")]
    [InlineData("11222333000181", "11222333000181")]
    [InlineData("00.000.000/0000-00", "00000000000000")]
    [InlineData("  11.222.333/0001-81  ", "11222333000181")]
    public void Sanitize_RemovesPunctuation(string input, string expected)
    {
        // Act
        var result = CnpjHelper.Sanitize(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Sanitize_ReturnsEmpty_WhenInputIsNullOrWhitespace(string? input)
    {
        // Act
        var result = CnpjHelper.Sanitize(input);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("11222333000181", true)]
    [InlineData("11.222.333/0001-81", true)]
    [InlineData("00000000000000", false)]
    [InlineData("11111111111111", false)]
    [InlineData("12345", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("1234567890123456", false)]
    public void IsValid_ReturnsExpectedResult(string? cnpj, bool expected)
    {
        // Act
        var result = CnpjHelper.IsValid(cnpj);

        // Assert
        Assert.Equal(expected, result);
    }
}
