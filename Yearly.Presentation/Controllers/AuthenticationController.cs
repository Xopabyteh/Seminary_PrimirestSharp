using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Application.Authentication.Queries.PrimirestUser;
using Yearly.Contracts;

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
        var loginQuery = new LoginQuery(request.Username, request.Password);
        var loginResult = await _mediator.Send(loginQuery);

        if (loginResult.IsError)
            return Problem(loginResult.Errors);

        var primirestUserQuery = new PrimirestUserQuery(loginResult.Value.SessionCookie);
        var primirestUserResult = await _mediator.Send(primirestUserQuery);

        return primirestUserResult.Match(
            user => Ok(new LoginResponse(user.Id, user.Username, loginResult.Value.SessionCookie)),
            Problem
            );
    }
}