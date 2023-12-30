using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Components.Pages.Auth;

public partial class LoginPage
{
    [Inject] private AuthenticationFacade AuthenticationFacade { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;

    [SupplyParameterFromForm] public string ModelUsername { get; set; } = string.Empty;

    [SupplyParameterFromForm] public string ModelPassword { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
    }

    private async void SubmitLogin()
    {
        var request = new LoginRequest(ModelUsername, ModelPassword);

        var loginResult = await AuthenticationFacade.LoginAsync(request);
        if (loginResult == default)
        {
            //Todo:
            return;
        }

        //Successful login
        await AuthService.SetSessionAsync(loginResult);

        NavigationManager.NavigateTo("/orders");
    }
}