using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Yearly.Application.Menus.Commands;
using Yearly.Presentation.OutputCaching;

namespace Yearly.Presentation.BackgroundServices;
public class PersistAvailableMenusBackgroundService : BackgroundService
{
    private readonly ILogger<PersistAvailableMenusBackgroundService> _logger;
    private readonly PeriodicTimer _periodicTimer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOutputCacheStore _outputCacheStore;
    public PersistAvailableMenusBackgroundService(ILogger<PersistAvailableMenusBackgroundService> logger, IServiceScopeFactory serviceScopeFactory, IOutputCacheStore outputCacheStore)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _outputCacheStore = outputCacheStore;
        _periodicTimer = new(TimeSpan.FromDays(1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mediator = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ISender>();

        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Persisting available menus");
            await mediator.Send(new PersistAvailableMenusCommand(), stoppingToken);

            //Evict old available menus cache
            await _outputCacheStore.EvictByTagAsync(OutputCacheTagName.GetAvailableMenusTag, stoppingToken);
        }
    }
}