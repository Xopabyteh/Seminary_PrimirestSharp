﻿using MapsterMapper;
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
        var result = await _mediator.Send(loginQuery);

        if (result.IsError)
            return Problem(result.Errors);

        // Add session cookie to cookies
        Response.Cookies.Append(SessionCookieDetails.Name, result.Value.SessionCookie, new CookieOptions
        {
            //SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = result.Value.SessionExpirationTime
        });

        var response = new LoginResponse(
            InitialActiveUserId: result.Value.ActiveLoggedUser.Id.Value,
            AvailableUserDetails: result.Value.AvailableUsers
                .Select(u => new UserDetailsResponse(
                    Username: u.Username,
                    UserId: u.Id.Value,
                    Roles: u.Roles
                        .Select(r => new UserRoleDTO(r.RoleCode))
                        .ToList()))
                .ToArray(),
            SessionCookieDetails: new(result.Value.SessionCookie, result.Value.SessionExpirationTime));

        return Ok(response);
    }

    [HttpPost("switch-context")]
    public Task<IActionResult> SwitchContext([FromQuery] int newUserId)
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var command = new SwitchPrimirestContextCommand(issuer.SessionCookie, new UserId(newUserId));
            var newSessionExpirationTimeResult = await _mediator.Send(command);

            return newSessionExpirationTimeResult.Match(
                value => Ok(new SwitchContextResponse(value)),
                Problem);
        });
    }

    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var logoutQuery = new LogoutCommand(issuer.SessionCookie);
            await _mediator.Send(logoutQuery);

            Response.Cookies.Delete(SessionCookieDetails.Name);

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