using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Yearly.Presentation.Controllers;

[Route("food")]
public class FoodController : ApiController
{
    private readonly ISender _mediator;

    public FoodController(ISender mediator)
    {
        _mediator = mediator;
    }

}
