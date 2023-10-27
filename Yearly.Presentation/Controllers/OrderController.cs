using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Orders.Commands;
using Yearly.Application.Orders.Queries;
using Yearly.Contracts.Order;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.MenuForWeekAgg;

namespace Yearly.Presentation.Controllers;

[Route("order")]
public class OrderController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public OrderController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("new-order")]
    public async Task<IActionResult> OrderFood([FromBody] OrderFoodRequest request)
    {
        var command = _mapper.Map<OrderFoodCommand>(request);
        var result = await _mediator.Send(command);
        
        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrdersForWeek(
        [FromQuery] string sessionCookie,
        [FromQuery] int menuForWeekId)
    {
        var orders = await _mediator.Send(new GetOrdersForWeekQuery(sessionCookie, new MenuForWeekId(menuForWeekId)));
        if (orders.IsError)
            return Problem(orders.Errors);

        var ordersResponse = new List<OrderResponse>(orders.Value.Count);
        foreach (var order in orders.Value)
        {
            ordersResponse.Add(new OrderResponse(
                order.OrderItemId,
                order.OrderId,
                order.FoodItemId));
        }

        var response = new MyOrdersResponse(ordersResponse);
        return Ok(response);
    }
}