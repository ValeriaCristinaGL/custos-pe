using TransparenciaPE.Application.Helpers;

namespace TransparenciaPE.UnitTests.Helpers;

public class CnpjHelperTests
{
    [Theory]
    [InlineData("11.222.333/0001-81", "11222333000181")]
    [InlineData("00.000.000/0000-00", "00000000000000")]
    public void Sanitize_ReturnsDigitsOnly_WhenInputHasPunctuation(string input, string expected)
    {
        // Act
        var result = CnpjHelper.Sanitize(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Sanitize_ReturnsSameValue_WhenInputHasOnlyDigits()
    {
        // Act
        var result = CnpjHelper.Sanitize("11222333000181");

        // Assert
        Assert.Equal("11222333000181", result);
    }

    [Fact]
    public void Sanitize_ReturnsDigitsOnly_WhenInputHasSurroundingWhitespace()
    {
        // Act
        var result = CnpjHelper.Sanitize("  11.222.333/0001-81  ");

        // Assert
        Assert.Equal("11222333000181", result);
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
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    public void IsValid_ReturnsTrue_WhenCnpjIsValid(string? cnpj)
    {
        // Act
        var result = CnpjHelper.IsValid(cnpj);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("12345")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("1234567890123456")]
    public void IsValid_ReturnsFalse_WhenCnpjIsInvalid(string? cnpj)
    {
        // Act
        var result = CnpjHelper.IsValid(cnpj);

        // Assert
        Assert.False(result);
    }
}
