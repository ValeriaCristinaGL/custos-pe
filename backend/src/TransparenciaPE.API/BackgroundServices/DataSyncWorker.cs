using TransparenciaPE.Application.Interfaces;

namespace TransparenciaPE.API.BackgroundServices;

/// <summary>
/// Background Worker that periodically syncs data from the external PE Transparency API
/// into the local PostgreSQL database.
/// </summary>
public class DataSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataSyncWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(6);

    public DataSyncWorker(IServiceProvider serviceProvider, ILogger<DataSyncWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DataSyncWorker started. Sync interval: {Interval}", _interval);

        // Initial sync on startup
        await SyncDataAsync();

        using var timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SyncDataAsync();
        }
    }

    private async Task SyncDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting periodic data sync...");

            using var scope = _serviceProvider.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<IDataSyncService>();

            var currentYear = DateTime.UtcNow.Year;
            var result = await syncService.SyncAllAsync(currentYear);

            _logger.LogInformation(
                "Data sync completed. Empenhos: {Empenhos}, Contratos: {Contratos}",
                result.EmpenhosProcessados,
                result.ContratosProcessados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data sync");
        }
    }
}
