using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Orders.Commands;
using Yearly.Application.Orders.Queries;
using Yearly.Contracts.Order;
using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("api/order")]
public class OrderController : ApiController
{
    private readonly IMapper _mapper;

    public OrderController(ISender mediator, IMapper mapper)
        : base(mediator)
    {
        _mapper = mapper;
    }

    [HttpPost("new-order")]
    public async Task<IActionResult> NewOrder([FromBody] NewOrderRequest request, [FromHeader] string sessionCookie)
    {
        var command = _mapper.Map<OrderFoodCommand>((request, sessionCookie));
        var result = await _mediator.Send(command);
        
        return result.Match(
            value => Ok(_mapper.Map<PrimirestOrderDataDTO>(value)),
            Problem);
    }

    [HttpPost("cancel-order")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request, [FromHeader] string sessionCookie)
    {
        var command = _mapper.Map<CancelOrderCommand>((request, sessionCookie));
        var result = await _mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrdersForWeek(
        [FromHeader] string sessionCookie,
        [FromQuery] int menuForWeekId)
    {
        var orders = await _mediator.Send(new GetOrdersForWeekQuery(sessionCookie, new WeeklyMenuId(menuForWeekId)));
        if (orders.IsError)
            return Problem(orders.Errors);

        var ordersResponse = new List<OrderDTO>(orders.Value.Count);
        foreach (var order in orders.Value)
        {
            ordersResponse.Add(_mapper.Map<OrderDTO>(order));
        }

        var response = new MyOrdersResponse(ordersResponse);
        return Ok(response);
    }

    [HttpGet("my-balance")]
    public Task<IActionResult> GetMyBalanceWithoutOrdersAccounted([FromHeader] string sessionCookie)
    {
        return PerformAuthenticatedActionAsync(sessionCookie, async (user) =>
        {
            var balanceResult =
                await _mediator.Send(new GetBalanceOfUserWithoutOrdersAccountedQuery(sessionCookie, user.Id));

            return balanceResult.Match(
                value => Ok(new MyBalanceResponse(value.Value)),
                Problem);
        });
    }
}