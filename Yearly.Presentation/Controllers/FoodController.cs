using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Foods.Commands;

namespace Yearly.Presentation.Controllers;
public class FoodController : ApiController
{
    private readonly ISender _mediator;

    public FoodController(ISender mediator)
    {
        _mediator = mediator;
    }


    [HttpPost("food/force")]
    public async Task<IActionResult> ForcePersistFoodsFromExternalService(string sessionCookie)
    {
        //Todo: authentication

        var result = await _mediator.Send(new PersistFoodsFromExternalServiceCommand());
        return result.Match(
            value => Ok(),
            Problem);
    }
}
