using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Yearly.Application.Foods.Queries;
using Yearly.Application.Menus.Commands;
using Yearly.Application.Menus.Queries;
using Yearly.Application.Orders.Queries;
using Yearly.Application.Photos.Queries;
using Yearly.Contracts.Common;
using Yearly.Contracts.Foods;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.MenuForWeekAgg;
using Yearly.Presentation.OutputCaching;
using static Yearly.Application.Errors.Errors;

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
        var menusForWeeks = await _mediator.Send(new GetAvailableMenusForWeeksQuery());
        //foreach (var menu in menus)
        //{
        //    var orders = await _mediator.Send(new GetOrdersForWeekQuery(request.SessionCookie, menu.Id));
        //    if(orders.IsError)
        //        return Problem(orders.Errors);

        //    orders.Value[0].
        //}

        var menusForWeeksResponse = new List<MenuForWeekResponse>();
        foreach (var menuForWeek in menusForWeeks)
        {
            var menusForDaysResponse = new List<MenuForDayResponse>();
            foreach (var menuForDay in menuForWeek.MenusForDays)
            {
                var foodsResponse = new List<FoodResponse>();
                foreach (var foodForDayId in menuForDay.FoodIds)
                {
                    var foodForDay = await _mediator.Send(new GetFoodQuery(foodForDayId));

                    var photoLinks = await _mediator.Send(new PhotosForFoodQuery(foodForDay.Id));

                    foodsResponse.Add(new(
                        foodForDay.Name,
                        foodForDay.Allergens,
                        photoLinks.Select(p => p.Link).ToList(),
                        _mapper.Map<PrimirestFoodIdentifierResponse>(foodForDay.PrimirestFoodIdentifier)));
                }

                menusForDaysResponse.Add(new MenuForDayResponse(menuForDay.Date, foodsResponse));
            }

            menusForWeeksResponse.Add(new MenuForWeekResponse(menusForDaysResponse, menuForWeek.Id.Value));
        }

        var response = new AvailableMenusResponse(menusForWeeksResponse);

        return Ok(response);
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