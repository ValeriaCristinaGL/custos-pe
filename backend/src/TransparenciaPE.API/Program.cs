using Microsoft.EntityFrameworkCore;
using TransparenciaPE.API.BackgroundServices;
using TransparenciaPE.API.Extensions;
using TransparenciaPE.API.Middlewares;
using TransparenciaPE.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Services configuration via extension methods
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddRateLimitingPolicies();
builder.AddSerilogLogging();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Background Worker for data sync
builder.Services.AddHostedService<DataSyncWorker>();

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Transparência PE v1");
        c.DocumentTitle = "Transparência PE - Swagger";
    });
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();

// Required for integration tests with WebApplicationFactory
public partial class Program { }
