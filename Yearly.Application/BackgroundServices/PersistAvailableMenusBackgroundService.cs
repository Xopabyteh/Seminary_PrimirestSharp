using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Yearly.Application.Menus.Commands;

namespace Yearly.Application.BackgroundServices;

public class PersistAvailableMenusBackgroundService : BackgroundService
{
    private readonly ILogger<PersistAvailableMenusBackgroundService> _logger;
    private readonly PeriodicTimer _periodicTimer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PersistAvailableMenusBackgroundService(ILogger<PersistAvailableMenusBackgroundService> logger, ISender mediator, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _periodicTimer = new(TimeSpan.FromDays(1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mediator = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ISender>();

        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Persisting available menus");
            await mediator.Send(new PersistAvailableMenusCommand(), stoppingToken);
        }
    }
}