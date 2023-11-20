using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Yearly.Application.Menus.Commands;
using Yearly.Presentation.OutputCaching;

namespace Yearly.Presentation.BackgroundJobs;
public class PersistAvailableMenusJob
{
    private readonly ILogger<PersistAvailableMenusJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOutputCacheStore _outputCacheStore;
    public PersistAvailableMenusJob(ILogger<PersistAvailableMenusJob> logger, IServiceScopeFactory serviceScopeFactory, IOutputCacheStore outputCacheStore)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _outputCacheStore = outputCacheStore;
    }

    public async Task ExecuteAsync()
    {
        var mediator = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ISender>();

        _logger.LogInformation("Persisting available menus");
        await mediator.Send(new PersistAvailableMenusCommand());

        //Evict old available menus cache
        await _outputCacheStore.EvictByTagAsync(OutputCacheTagName.GetAvailableMenusTag, CancellationToken.None);
    }
}