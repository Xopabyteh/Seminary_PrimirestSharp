﻿using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Yearly.Application.Foods.Queries;
using Yearly.Application.Menus.Commands;
using Yearly.Application.Menus.Queries;
using Yearly.Application.Photos.Queries.ForFood;
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
        var weeklyMenus = await _mediator.Send(new GetAvailableWeeklyMenusQuery());
        //foreach (var menu in menus)
        //{
        //    var orders = await _mediator.Send(new GetOrdersForWeekQuery(request.SessionCookie, menu.Id));
        //    if(orders.IsError)
        //        return Problem(orders.Errors);

        //    orders.Value[0].
        //}

        //Todo: update docs new food identifier
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

                    var photoLinks = await _mediator.Send(new PhotosForFoodQuery(foodForDay.Id));

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