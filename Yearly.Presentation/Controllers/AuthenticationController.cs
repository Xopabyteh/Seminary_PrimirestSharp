using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Authentication.Commands.Login;
using Yearly.Application.Authentication.Commands.Roles;
using Yearly.Application.Authentication.Queries.Logout;
using Yearly.Contracts.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public AuthenticationController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        //var loginQuery = new LoginQuery(request.Username, request.Password);
        var loginQuery = _mapper.Map<LoginCommand>(request);
        var loginResult = await _mediator.Send(loginQuery);

        if (loginResult.IsError)
            return Problem(loginResult.Errors);

        return loginResult.Match(
            value => Ok(_mapper.Map<LoginResponse>(value)),
            Problem);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromHeader] string sessionCookie)
    {
        var logoutQuery = new LogoutQuery(sessionCookie);
        await _mediator.Send(logoutQuery);
        return Ok();
    }

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole(
        [FromBody] AddRoleRequest request,
        [FromHeader] string sessionCookie)
    {
        var command = new AddRoleToUserCommand(sessionCookie, new UserId(request.UserId), new UserRole(request.RoleCode));
        var result = await _mediator.Send(command);
        return result.Match(
            _ => Ok(),
            Problem);
    }
}