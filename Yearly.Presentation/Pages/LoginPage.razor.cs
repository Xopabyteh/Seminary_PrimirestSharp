using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Yearly.Contracts.Authentication;
using Yearly.Presentation.Http;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages;

public partial class LoginPage
{
    [Inject] private IHttpClientFactory _clientFactory { get; set; } = null!;
    [Inject] private SessionDetailsService _sessionDetailsService { get; set; } = null!;
    [Inject] private BrowserCookieService _browserCookieService { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    private LoginModel model = new();

    private async Task SubmitLogin()
    {
        var request = new LoginRequest(model.Username, model.Password);

        using var client = _clientFactory.CreateClient(HttpClientNames.SharpAPI);

        var result = await client.PostAsJsonAsync("/api/auth/login", request);

        if (!result.IsSuccessStatusCode)
        {
            var problem = await result.Content.ReadFromJsonAsync<ProblemDetails>();
            return;
        }

        // -> Successfully logged in
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();

        //Save cookie
        await _browserCookieService.WriteCookieAsync(
            SessionCookieDetails.Name,
            response.SessionCookieDetails.Value,
            response.SessionCookieDetails.ExpirationDate.Date);

        //Init session
        _sessionDetailsService.Init(response.SessionCookieDetails.Value, response.UserDetails);

        // -> Redirect to home page
        _navigationManager.NavigateTo("/");
    }

    private class LoginModel
    {
        [Required] public string Username { get; set; } = string.Empty;

        [Required] public string Password { get; set; } = string.Empty;
    }
}