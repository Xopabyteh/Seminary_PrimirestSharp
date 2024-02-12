using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Yearly.Contracts.Authentication;
using Yearly.Presentation.BlazorServer.Services;
using Yearly.Presentation.Http;

namespace Yearly.Presentation.BlazorServer.Components.Pages;

public partial class LoginPage
{
    [Inject] private IHttpClientFactory _clientFactory { get; set; } = null!;
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

        // -> Success
        
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();

        await _browserCookieService.WriteCookie(
            SessionCookieDetails.Name,
            response.SessionCookieDetails.Value,
            response.SessionCookieDetails.ExpirationDate.Date);

        // -> Redirect to home page
        _navigationManager.NavigateTo("/");
    }

    private class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}