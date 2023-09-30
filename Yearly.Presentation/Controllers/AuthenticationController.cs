using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Services.Authentication;
using Yearly.Contracts;

namespace Yearly.Presentation.Controllers;

[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly IPrimirestAuthService _primirestAuthService;

    public AuthenticationController(IPrimirestAuthService primirestAuthService)
    {
        _primirestAuthService = primirestAuthService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var loginResult = await _primirestAuthService.LoginAsync(request.Username, request.Password);
        var primirestUserResult = await _primirestAuthService.GetPrimirestUserInfoAsync(loginResult.SessionCookie);

        return primirestUserResult.Match(
            user => Ok(new LoginResponse(user.Id, user.Username, user.SessionCookie)),
            Problem
            );
    }
}