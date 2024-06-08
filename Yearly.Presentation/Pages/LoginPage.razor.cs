using System.ComponentModel.DataAnnotations;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using MediatR;
using Microsoft.AspNetCore.Components;
using Yearly.Application.Authentication.Commands;
using Yearly.Contracts.Authentication;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages;

public partial class LoginPage
{
    [Inject] private ISender _mediator { get; set; } = null!;
    [Inject] private SessionDetailsService _sessionDetailsService { get; set; } = null!;
    [Inject] private BrowserCookieService _browserCookieService { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private IHxMessengerService _messenger { get; set; } = null!;


    private LoginModel model = new();

    private async Task SubmitLogin()
    {
        var loginCommand = new LoginCommand(model.Username, model.Password);
        var result = await _mediator.Send(loginCommand);

        if (result.IsError)
        {
            _messenger.AddError(result.FirstError.Description);
            return;
        }

        //Save cookie
        await _browserCookieService.WriteCookieAsync(
            SessionCookieDetails.Name,
            result.Value.SessionCookie,
            result.Value.SessionExpirationTime.Date);

        //Init session
        _sessionDetailsService.Init(result.Value.SessionCookie, result.Value.ActiveLoggedUser);

        // -> Redirect to home page
        _navigationManager.NavigateTo("/");
    }

    private class LoginModel
    {
        [Required] public string Username { get; set; } = string.Empty;

        [Required] public string Password { get; set; } = string.Empty;
    }
}