using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Threading.RateLimiting;
using TransparenciaPE.Application.Interfaces;
using TransparenciaPE.Application.Services;
using TransparenciaPE.Domain.Entities;
using TransparenciaPE.Domain.Interfaces;
using TransparenciaPE.Infrastructure.Data;
using TransparenciaPE.Infrastructure.ExternalClients;
using TransparenciaPE.Infrastructure.QueryServices;
using TransparenciaPE.Infrastructure.Repositories;
using UoW = TransparenciaPE.Infrastructure.UnitOfWork;

namespace TransparenciaPE.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IPesquisaService, PesquisaService>();
        services.AddScoped<IDataSyncService, DataSyncService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IEmpenhoRepository, EmpenhoRepository>();
        services.AddScoped<IContratoRepository, ContratoRepository>();
        services.AddScoped<IUnitOfWork, UoW.UnitOfWork>();
        services.AddScoped<IDashboardQueryService, DapperDashboardQueryService>();
        services.AddScoped<IPEDataClient, FakePEDataClient>();

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API Transparência PE",
                Version = "v1",
                Description = "API de Transparência de Custos do Estado de Pernambuco — MCASP/LAI",
                Contact = new OpenApiContact
                {
                    Name = "Equipe de Desenvolvimento",
                    Email = "nathannmvr@gmail.com"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    }));
        });

        return services;
    }
}
