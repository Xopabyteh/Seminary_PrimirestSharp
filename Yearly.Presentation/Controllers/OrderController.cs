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
    public Task<IActionResult> NewOrder([FromBody] NewOrderRequest request)
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var command = _mapper.Map<OrderFoodCommand>((request, issuer.SessionCookie));
            var result = await _mediator.Send(command);

            return result.Match(
                value => Ok(_mapper.Map<PrimirestOrderDataDTO>(value)),
                Problem);
        });
    }

    [HttpPost("cancel-order")]
    public Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var command = _mapper.Map<CancelOrderCommand>((request, issuer.SessionCookie));
            var result = await _mediator.Send(command);

            return result.Match(
                _ => Ok(),
                Problem);
        });
    }

    [HttpGet("my-orders")]
    public Task<IActionResult> GetMyOrdersForWeek([FromQuery] int menuForWeekId)
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var orders = await _mediator.Send(new GetOrdersForWeekQuery(
                issuer.SessionCookie,
                new WeeklyMenuId(menuForWeekId)));

            if (orders.IsError)
                return Problem(orders.Errors);

            var ordersResponse = new List<OrderDTO>(orders.Value.Count);
            foreach (var order in orders.Value)
            {
                ordersResponse.Add(_mapper.Map<OrderDTO>(order));
            }

            var response = new MyOrdersResponse(ordersResponse);
            return Ok(response);
        });
    }

    [HttpGet("my-balance")]
    public Task<IActionResult> GetMyBalanceWithoutOrdersAccounted()
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var balanceResult =
                await _mediator.Send(new GetUserFinanceDetailsQuery(issuer.SessionCookie));

            return balanceResult.Match(
                value => Ok(new MyBalanceResponse(value.AccountBalance.Value, value.OrderedFor.Value)),
                Problem);
        });
    }
}