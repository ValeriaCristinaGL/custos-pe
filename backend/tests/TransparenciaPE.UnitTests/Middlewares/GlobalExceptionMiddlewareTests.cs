using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TransparenciaPE.API.Middlewares;
using TransparenciaPE.Domain.Exceptions;

namespace TransparenciaPE.UnitTests.Middlewares;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;

    public GlobalExceptionMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
    }

    /// <summary>
    /// Cria um DefaultHttpContext com corpo de resposta gravável (MemoryStream).
    /// </summary>
    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(response.Body).ReadToEndAsync();
    }

    [Fact]
    public async Task InvokeAsync_CallsNext_WhenNoExceptionOccurs()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200); // status não alterado
    }

    public static IEnumerable<object[]> ExceptionStatusCodeData =>
        new List<object[]>
        {
            new object[] { new ArgumentException("Termo de busca inválido."), 400 },
            new object[] { new NotFoundException("Recurso não encontrado."), 404 },
            new object[] { new InvalidOperationException("Falha inesperada interna."), 500 },
        };

    [Theory]
    [MemberData(nameof(ExceptionStatusCodeData))]
    public async Task InvokeAsync_ReturnsExpectedStatusCode_ForGivenException(Exception exception, int expectedStatusCode)
    {
        // Arrange
        RequestDelegate next = _ => throw exception;
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsJsonError_WhenExceptionOccurs()
    {
        // Arrange
        RequestDelegate next = _ => throw new ArgumentException("erro estruturado");
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.ContentType.Should().Contain("application/json");
        var body = await ReadBodyAsync(context.Response);
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.TryGetProperty("statusCode", out _).Should().BeTrue();
        doc.RootElement.TryGetProperty("message", out _).Should().BeTrue();
        doc.RootElement.TryGetProperty("timestamp", out _).Should().BeTrue();
    }
}
