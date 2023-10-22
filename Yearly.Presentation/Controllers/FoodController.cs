using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Foods.Commands;
using Yearly.Contracts.Foods;

namespace Yearly.Presentation.Controllers;

[Route("food")]
public class FoodController : ApiController
{
    private readonly ISender _mediator;

    public FoodController(ISender mediator)
    {
        _mediator = mediator;
    }


    [HttpPost("force")]
    public async Task<IActionResult> ForcePersistFoodsFromExternalService([FromBody] ForcePersistFoodsFromExternalServiceRequest request)
    {
        var result = await _mediator.Send(new PersistFoodsFromExternalServiceCommand());
        return result.Match(
            value => Ok(value),
            Problem);
    }
}
