using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Authentication.Queries;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Presentation.Http;

namespace Yearly.Presentation.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected readonly ISender _mediator;

    public ApiController(ISender mediator)
    {
        _mediator = mediator;
    }

    protected async Task<IActionResult> PerformAuthenticatedActionAsync(string sessionCookie, Func<User, Task<IActionResult>> action)
    {
        //Auth
        var userResult = await _mediator.Send(new UserBySessionQuery(sessionCookie));
        if (userResult.IsError)
            return Problem(userResult.Errors);

        return await action(userResult.Value);
    }

    protected Task<IActionResult> PerformAuthorizedActionAsync(
        string sessionCookie,
        Func<User, Task<IActionResult>> action, 
        params UserRole[] roles)
    {
        return PerformAuthenticatedActionAsync(sessionCookie, async user =>
        {
            if (!user.Roles.Any(roles.Contains))
                return Unauthorized();

            return await action(user);
        });
    }

    protected IActionResult Problem(List<Error> errors)
    {
        HttpContext.Items[HttpContextItemKeys.Errors] = errors;

        var firstError = errors[0];
        var statusCode = firstError.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: firstError.Description);
    }
}