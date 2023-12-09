using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Authentication.Commands;
using Yearly.Application.Authentication.Queries;
using Yearly.Contracts.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly IMapper _mapper;

    public AuthenticationController(ISender mediator, IMapper mapper) 
        : base(mediator)
    {
        _mapper = mapper;
    }

    [HttpGet("my-details")]
    public async Task<IActionResult> GetMyDetails([FromHeader] string sessionCookie)
    {
        var userResult = await _mediator.Send(new UserBySessionQuery(sessionCookie));

        return userResult.Match(
            value => Ok(_mapper.Map<UserDetailsResponse>(value)),
            Problem);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
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
        var logoutQuery = new LogoutCommand(sessionCookie);
        await _mediator.Send(logoutQuery);
        return Ok();
    }

    [HttpPost("add-role")]
    public Task<IActionResult> AddRole(
        [FromBody] RoleRequest request,
        [FromHeader] string sessionCookie)
    {
        return PerformAuthorizedActionAsync(
            sessionCookie,
            async _ =>
            {
                var command = new AddRoleToUserCommand(new UserId(request.UserId), new UserRole(request.RoleCode));
                var result = await _mediator.Send(command);
                return result.Match(
                    _ => Ok(),
                    Problem);
            },
            UserRole.Admin);
    }

    [HttpPost("remove-role")]
    public Task<IActionResult> RemoveRole(
        [FromBody] RoleRequest request,
        [FromHeader] string sessionCookie)

    {
        return PerformAuthorizedActionAsync(
            sessionCookie,
            async _ =>
            {
                var command = new RemoveRoleFromUserCommand(new UserId(request.UserId), new UserRole(request.RoleCode));
                var result = await _mediator.Send(command);
                return result.Match(
                    _ => Ok(),
                    Problem);
            },
            UserRole.Admin);
    }
}