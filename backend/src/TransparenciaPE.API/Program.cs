using Microsoft.EntityFrameworkCore;
using Npgsql;
using TransparenciaPE.API.BackgroundServices;
using TransparenciaPE.API.Extensions;
using TransparenciaPE.API.Middlewares;
using TransparenciaPE.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Services configuration via extension methods
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddRateLimitingPolicies(builder.Configuration);
builder.AddSerilogLogging();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS — permite comunicação frontend → backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Value?
            .Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new[] { "http://localhost:5173" };
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Background Worker for data sync
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<DataSyncWorker>();
}

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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FrontendPolicy");
app.UseRateLimiter();
app.MapControllers();

// Apply pending migrations on startup
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                break;
            }
            catch (NpgsqlException) when (attempt < maxRetries)
            {
                await Task.Delay(delay);
            }
        }

        // Seed com dados fictícios apenas em Development
        if (app.Environment.IsDevelopment())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
            await DbSeeder.SeedAsync(dbContext, logger);
        }
    }
}

app.Run();

// Required for integration tests with WebApplicationFactory
public partial class Program { }
