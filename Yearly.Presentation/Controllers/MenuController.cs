using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Application.Authentication.Queries.PrimirestUser;
using Yearly.Contracts;
using Yearly.Contracts.Authentication;
using Yearly.Contracts.Menu;

namespace Yearly.Presentation.Controllers;

public class MenuController : ApiController
{
    private readonly ISender _mediator;

    public MenuController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("menu")]
    public async Task<IActionResult> GetMenus(MenuRequest request)
    {
        return Ok();
    }
}