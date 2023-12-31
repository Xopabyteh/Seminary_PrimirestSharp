using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Auth;

public partial class LoginPage
{
    [Inject] private AuthenticationFacade AuthenticationFacade { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ToastService ToastService { get; set; } = null!;

    [SupplyParameterFromForm] public string ModelUsername { get; set; } = string.Empty;

    [SupplyParameterFromForm] public string ModelPassword { get; set; } = string.Empty;

    private bool isLoggingIn;

    private async void TrySubmitLogin()
    {
        isLoggingIn = true;
        StateHasChanged();

        if (!await ValidateModel())
        {
            isLoggingIn = false;
            StateHasChanged();

            return;
        }

        var request = new LoginRequest(ModelUsername, ModelPassword);

        var loginResult = await AuthenticationFacade.LoginAsync(request);
        if (loginResult.IsT1)
        {
            //Problem
            isLoggingIn = false;
            StateHasChanged();

            await ToastService.ShowErrorAsync(loginResult.AsT1.Title);
            return;
        }

        //Successful login
        await AuthService.SetSessionAsync(loginResult.AsT0);

        NavigationManager.NavigateTo("/orders");

        // No need to reset isLoggingIn anymore..
    }

    private async Task<bool> ValidateModel()
    {
        if (string.IsNullOrWhiteSpace(ModelUsername))
        {
            await ToastService.ShowErrorAsync("Vyplòte uživatelské jméno");
            return false;
        }

        if (string.IsNullOrWhiteSpace(ModelPassword))
        {
            await ToastService.ShowErrorAsync("Vyplòte heslo");
            return false;
        }

        return true;
    }
}