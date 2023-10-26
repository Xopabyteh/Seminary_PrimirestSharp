using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Orders.Commands;
using Yearly.Contracts.Order;

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
}