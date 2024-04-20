using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Yearly.Application.Menus.Commands;
using Yearly.Application.Menus.Queries;
using Yearly.Contracts.Menu;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Presentation.OutputCaching;

namespace Yearly.Presentation.Controllers;

[Route("api/menu")]
public class MenuController : ApiController
{
    private readonly IOutputCacheStore _outputCacheStore;
    public MenuController(ISender mediator, IOutputCacheStore outputCacheStore) 
        : base(mediator)
    {
        _outputCacheStore = outputCacheStore;
    }

    [HttpGet("available")]
    [OutputCache(PolicyName = OutputCachePolicyName.GetAvailableMenus)]
    public async Task<IActionResult> GetAvailableMenus()
    {
        var result = await _mediator.Send(new GetAvailableMenuDTOsQuery());

        var response = new AvailableMenusResponse(result);
        return Ok(response);
    }

    [HttpPost("force")]
    public Task<IActionResult> ForceAvailablePersistMenusFromPrimirest()
    {
        return PerformAuthorizedActionAsync(
            async _ =>
            {
                var command = new PersistAvailableMenusCommand();
                var result = await _mediator.Send(command);

                return result.Match(
                    _ =>
                    {
                        //Evict old available menus cache
                        _outputCacheStore.EvictByTagAsync(OutputCacheTagName.GetAvailableMenusTag, CancellationToken.None);
                        
                        return Ok();
                    },
                    Problem);
            }, 
            UserRole.Admin);
    }
}