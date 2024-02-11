using MediatR;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Yearly.Application.Authentication.Commands;

namespace Yearly.Presentation.BlazorServer.Components.Pages;

public partial class LoginPage
{
    private readonly ISender _mediator;
    private readonly CircuitHandler _circuitHandler;

    private LoginModel model = new();

    public LoginPage(ISender mediator, CircuitHandler circuitHandler)
    {
        _mediator = mediator;
        _circuitHandler = circuitHandler;
    }

    private async Task SubmitLogin()
    {
        var command = new LoginCommand(model.Username, model.Password);
        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            //Todo
            return;
        }

        //- > Success
        //Todo: ...
    }

    private class LoginModel
    {
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}