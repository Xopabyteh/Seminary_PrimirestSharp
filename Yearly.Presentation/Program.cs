using Microsoft.AspNetCore.Mvc;
using Yearly.Application;
using Yearly.Application.Services.Authentication;
using Yearly.Contracts;
using Yearly.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddApplication()
        .AddInfrastructure();

}


var app = builder.Build();
{
    app.MapPost("/login", async (
        [FromBody] LoginRequest request,
        [FromServices] IPrimirestAuthService primirestAuthService) =>
    {
        var loginResult = await primirestAuthService.LoginAsync(request.Username, request.Password);
        var primirestUser = await primirestAuthService.GetPrimirestUserInfoAsync(loginResult.SessionCookie);

        var response = new LoginResponse(primirestUser.Id, primirestUser.Username, loginResult.SessionCookie);

        return Results.Ok(response);
    });

    app.Run();
}

