using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Application.Authentication.Queries.PrimirestUser;
using Yearly.Contracts;
using Yearly.Contracts.Authentication;

namespace Yearly.Presentation.Controllers;

[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        //TODO: Mapping & Menu controller

        var loginQuery = new LoginQuery(request.Username, request.Password);
        var loginResult = await _mediator.Send(loginQuery);

        if (loginResult.IsError)
            return Problem(loginResult.Errors);

        var userQuery = new UserQuery(loginResult.Value.SessionCookie);
        var userResult = await _mediator.Send(userQuery);

        return userResult.Match(
            user => Ok(new LoginResponse(user.Id.Value.ToString(), user.Username, loginResult.Value.SessionCookie)),
            Problem
            );
    }
}