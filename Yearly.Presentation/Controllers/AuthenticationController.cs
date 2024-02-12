using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Authentication.Commands;
using Yearly.Application.Authentication.Queries;
using Yearly.Contracts.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("api/auth")]
public class AuthenticationController : ApiController
{
    private readonly IMapper _mapper;

    public AuthenticationController(ISender mediator, IMapper mapper) 
        : base(mediator)
    {
        _mapper = mapper;
    }

    [HttpGet("my-details")]
    public Task<IActionResult> GetMyDetails()
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var userResult = await _mediator.Send(new UserBySessionQuery(issuer.SessionCookie));

            return userResult.Match(
                value => Ok(_mapper.Map<UserDetailsResponse>(value)),
                Problem);
        });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var loginQuery = _mapper.Map<LoginCommand>(request);
        var loginResult = await _mediator.Send(loginQuery);

        if (loginResult.IsError)
            return Problem(loginResult.Errors);

        //Add session cookie to cookies
        Response.Cookies.Append(SessionCookieDetails.Name, loginResult.Value.SessionCookie, new CookieOptions
        {
            //SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = loginResult.Value.SessionExpirationTime
        });

        return Ok(_mapper.Map<LoginResponse>(loginResult.Value));
    }

    //[HttpPost("logout")]
    //public async Task<IActionResult> Logout([FromHeader] string sessionCookie)
    //{
    //    var logoutQuery = new LogoutCommand(sessionCookie);
    //    await _mediator.Send(logoutQuery);
    //    return Ok();
    //}

    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var logoutQuery = new LogoutCommand(issuer.SessionCookie);
            await _mediator.Send(logoutQuery);

            Response.Cookies.Delete("session");

            return Ok();
        });
    }

    [HttpPost("add-role")]
    public Task<IActionResult> AddRole(
        [FromBody] RoleRequest request)
    {
        return PerformAuthorizedActionAsync(
            async issuer =>
            {
                var command = new AddRoleToUserCommand(issuer.User, new UserId(request.UserId), new UserRole(request.RoleCode));
                var result = await _mediator.Send(command);
                return result.Match(
                    _ => Ok(),
                    Problem);
            },
            UserRole.Admin);
    }

    [HttpPost("remove-role")]
    public Task<IActionResult> RemoveRole(
        [FromBody] RoleRequest request)

    {
        return PerformAuthorizedActionAsync(
            async issuer =>
            {
                var command = new RemoveRoleFromUserCommand(issuer.User, new UserId(request.UserId), new UserRole(request.RoleCode));
                var result = await _mediator.Send(command);
                return result.Match(
                    _ => Ok(),
                    Problem);
            },
            UserRole.Admin);
    }
}