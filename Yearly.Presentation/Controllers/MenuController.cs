using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Yearly.Application.Foods.Queries;
using Yearly.Application.Menus.Commands;
using Yearly.Application.Menus.Queries;
using Yearly.Application.Photos.Queries;
using Yearly.Contracts.Common;
using Yearly.Contracts.Menu;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Presentation.OutputCaching;

namespace Yearly.Presentation.Controllers;

[Route("menu")]
public class MenuController : ApiController
{
    private readonly IMapper _mapper;
    private readonly IOutputCacheStore _outputCacheStore;

    public MenuController(IMapper mapper, ISender mediator, IOutputCacheStore outputCacheStore) 
        : base(mediator)
    {
        _mapper = mapper;
        _outputCacheStore = outputCacheStore;
    }

    [HttpGet("available")]
    [OutputCache(PolicyName = OutputCachePolicyName.GetAvailableMenus)]
    public async Task<IActionResult> GetAvailableMenus()
    {
        var weeklyMenus = await _mediator.Send(new GetAvailableWeeklyMenusQuery());

        //Todo: optimize this mess

        var weeklyMenusResponse = new List<WeeklyMenuResponse>();
        foreach (var weeklyMenu in weeklyMenus)
        {
            var dailyMenusResponse = new List<DailyMenuResponse>();
            foreach (var dailyMenu in weeklyMenu.DailyMenus)
            {
                var foodsResponse = new List<FoodResponse>();
                foreach (var foodForDayId in dailyMenu.FoodIds)
                {
                    var foodForDay = await _mediator.Send(new GetFoodQuery(foodForDayId));

                    var photoLinks = await _mediator.Send(new PhotosForFoodQuery(foodForDay));

                    foodsResponse.Add(new(
                        foodForDay.Name,
                        foodForDay.Allergens,
                        photoLinks.Select(p => p.Link).ToList(),
                        foodForDay.Id.Value,
                        _mapper.Map<PrimirestFoodIdentifierContract>(foodForDay.PrimirestFoodIdentifier)));
                }

                dailyMenusResponse.Add(new DailyMenuResponse(dailyMenu.Date, foodsResponse));
            }

            weeklyMenusResponse.Add(new WeeklyMenuResponse(dailyMenusResponse, weeklyMenu.Id.Value));
        }

        var response = new AvailableMenusResponse(weeklyMenusResponse);

        return Ok(response);
    }

    [HttpPost("force")]
    public Task<IActionResult> ForceAvailablePersistMenusFromPrimirest([FromHeader] string sessionCookie)
    {
        return PerformAuthorizedActionAsync(
            sessionCookie,
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