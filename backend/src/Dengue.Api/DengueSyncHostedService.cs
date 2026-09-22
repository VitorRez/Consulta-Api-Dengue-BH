using Dengue.Application.Interfaces;

namespace Dengue.Api;

public class DengueSyncHostedService : BackgroundService{
    private readonly IServiceProvider _services;
    private readonly ILogger<DengueSyncHostedService> _logger;

    public DengueSyncHostedService(IServiceProvider services, ILogger<DengueSyncHostedService> logger){
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken){
        //Sincroniza ao iniciar o servidor
        await RunStartupSyncAsync(stoppingToken);

        //Sincroniza toda segunda as 3h
        while (!stoppingToken.IsCancellationRequested){
            var nextRun = CalculateNextMonday(DateTime.UtcNow);
            var delay = nextRun - DateTime.UtcNow;

            _logger.LogInformation(
                "Próximo sync agendado para {NextRun} UTC (em {Delay}).",
                nextRun, delay);

            try{
                await Task.Delay(delay, stoppingToken);
            }catch (TaskCanceledException){
                return;
            }

            await RunScheduledSyncAsync(stoppingToken);
        }
    }

    private async Task RunStartupSyncAsync(CancellationToken cancellationToken){
        using var scope = _services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IDengueRepository>();
        var service = scope.ServiceProvider.GetRequiredService<IDengueService>();

        try{
            var existing = await repository.GetLastWeeksAsync(1, cancellationToken);

            if (existing.Count > 0){
                _logger.LogInformation("Banco já populado. Pulando sync de startup.");
                return;
            }

            _logger.LogInformation("Banco vazio. Iniciando sync de startup...");
            var count = await service.SyncLastSixMonthsAsync(cancellationToken);
            _logger.LogInformation("Sync de startup concluído: {Count} registros.", count);
        }catch (Exception ex){
            _logger.LogError(ex, "Falha no sync de startup. A API continuará subindo.");
        }
    }

    private async Task RunScheduledSyncAsync(CancellationToken cancellationToken){
        using var scope = _services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IDengueService>();

        try{
            _logger.LogInformation("Iniciando sync agendado...");
            var count = await service.SyncLastSixMonthsAsync(cancellationToken);
            _logger.LogInformation("Sync agendado concluído: {Count} registros.", count);
        }catch (Exception ex){
            _logger.LogError(ex, "Falha no sync agendado.");
        }
    }

    private static DateTime CalculateNextMonday(DateTime fromUtc){
        // Próxima segunda às 03:00 UTC
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)fromUtc.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0 && fromUtc.TimeOfDay > new TimeSpan(3, 0, 0))daysUntilMonday = 7;

        return fromUtc.Date.AddDays(daysUntilMonday).AddHours(3);
    }
}