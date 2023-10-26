using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Yearly.Application.Foods.Queries;
using Yearly.Application.Menus.Commands;
using Yearly.Application.Menus.Queries;
using Yearly.Application.Photos.Queries;
using Yearly.Contracts.Common;
using Yearly.Contracts.Foods;
using Yearly.Contracts.Menu;
using Yearly.Presentation.OutputCaching;

namespace Yearly.Presentation.Controllers;

[Route("menu")]
public class MenuController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public MenuController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("available")]
    [OutputCache(PolicyName = OutputCachePolicyName.GetAvailableMenus)]
    public async Task<IActionResult> GetAvailableMenus()
    {
        ////Todo: maybe not load all foods at once? or maybe yes, whats more optimal?
        var menus = await _mediator.Send(new AvailableMenusQuery());

        var foodsForMenus = await _mediator.Send(new FoodsForMenusQuery(menus));

        var photosForFood = await _mediator.Send(new PhotosForFoodsQuery(foodsForMenus));

        //This might be slow, but because this will be cached and invoked once per week, it should be fine
        var menuResponses = menus.Select(m =>
        {
            return new MenuForDayResponse(m.Date, m.FoodIds.Select(mFId =>
                {
                    var foodForMenu = foodsForMenus.Single(f => f.Id == mFId);
                    return new FoodResponse(
                        foodForMenu.Name,
                        foodForMenu.Allergens,
                        photosForFood.Where(p => p.FoodId == mFId).Select(p => p.Link).ToList(),
                        _mapper.Map<PrimirestOrderIdentifierResponse>(foodForMenu.PrimirestOrderIdentifier));
                }).ToList());
        }).ToList();

        var availableMenusResponse = new AvailableMenusResponse(menuResponses);
        return Ok(availableMenusResponse);
    }

    [HttpPost("force")]
    public async Task<IActionResult> ForceAvailablePersistMenusFromPrimirest([FromBody] ForcePersistAvailableMenusFromPrimirestRequest request)
    {
        var command = _mapper.Map<PersistAvailableMenusCommand>(request);
        var result = await _mediator.Send(command);

        //Revoke old cache
        //Todo: 

        return result.Match(
            _ => Ok(),
            Problem);
    }
}