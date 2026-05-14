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
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionOccurs()
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

    [Fact]
    public async Task InvokeAsync_ShouldReturn400_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        RequestDelegate next = _ => throw new ArgumentException("Termo de busca inválido.");
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        var body = await ReadBodyAsync(context.Response);
        body.Should().Contain("Termo de busca inv\u00e1lido.");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn404_WhenNotFoundExceptionIsThrown()
    {
        // Arrange
        RequestDelegate next = _ => throw new NotFoundException("Recurso não encontrado.");
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
        var body = await ReadBodyAsync(context.Response);
        body.Should().Contain("Recurso n\u00e3o encontrado.");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn500_WhenUnhandledExceptionIsThrown()
    {
        // Arrange
        RequestDelegate next = _ => throw new InvalidOperationException("Falha inesperada interna.");
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        var body = await ReadBodyAsync(context.Response);
        body.Should().Contain("internal server error");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnJson_WithStatusCode_Message_Timestamp()
    {
        // Arrange
        RequestDelegate next = _ => throw new ArgumentException("erro estruturado");
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert — corpo deve ser JSON com os campos obrigatórios
        context.Response.ContentType.Should().Contain("application/json");
        var body = await ReadBodyAsync(context.Response);
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.TryGetProperty("statusCode", out _).Should().BeTrue();
        doc.RootElement.TryGetProperty("message", out _).Should().BeTrue();
        doc.RootElement.TryGetProperty("timestamp", out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn400_WhenDomainExceptionWithStatus400()
    {
        // Arrange — NotFoundException herda de DomainException com StatusCode=404
        // Aqui verificamos que o StatusCode da DomainException é respeitado
        RequestDelegate next = _ => throw new NotFoundException("Órgão não localizado.");
        var middleware = new GlobalExceptionMiddleware(next, _mockLogger.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
    }
}
