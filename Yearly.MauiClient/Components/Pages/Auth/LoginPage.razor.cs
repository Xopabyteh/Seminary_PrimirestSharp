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

    private bool isLoggingIn = false;
    private bool isSettingUpAutoLogin = false;

    /// <summary>
    /// Check client docs to better see what's happening here :)
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //Make sure auto login is loaded
        //(don't care if active, but loaded, so we know the state of it)
        await AuthService.EnsureAutoLoginStateLoaded();

        //Are we setting up autologin?
        var pageUri = NavigationManager.Uri;
        isSettingUpAutoLogin = pageUri.Contains("setupAutoLogin");

        if (isSettingUpAutoLogin)
        {
            await ToastService.ShowInformationAsync("P�ihla� se znovu pro nastaven� Auto Loginu",  durationMillis: -1);
            return; //We don't want to load a session and continue,
                    // but wait for login, so we can setup Auto Login
        }

        //1. Try get session
        var hasSession = await AuthService.TryLoadStoredSessionAsync();
        if (hasSession)
        {
            //We already have a session, skip login
            NavigationManager.NavigateTo("/orders");
            return;
        }

        //2. Try get stored credentials
        // Did we come from logging out?
        // If we just logged out, don't auto login back in,
        // but rather prefill the form
        var didComeFromLogout = pageUri.Contains("loginFromLogout");
        if (AuthService.AutoLoginStoredCredentials is not null)
        {
            if (didComeFromLogout)
            {
                //Prefill form
                ModelUsername = AuthService.AutoLoginStoredCredentials.Username;
                ModelPassword = AuthService.AutoLoginStoredCredentials.Password;
                StateHasChanged();
                return; //If we just logged out, don't auto login back in
            }

            //Try to Auto Login
            var loginResult = await AuthenticationFacade.LoginAsync(AuthService.AutoLoginStoredCredentials);
            if (loginResult.IsT1)
            {
                //Problem
                isLoggingIn = false;
                StateHasChanged();

                await ToastService.ShowErrorAsync("Auto login se nezda�il, m�nil/a jste si heslo?");
                return;
            }
            // -> Successful login

            await AuthService.SetSessionAsync(loginResult.AsT0);
            NavigationManager.NavigateTo("/orders");
        }

        //3. Try get credentials from user ...
    }

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
        // -> Successful login

        if (isSettingUpAutoLogin)
        {
            //We are setting up autologin, so we don't want to login the user, but just store the credentials
            await ToastService.ShowSuccessAsync("Auto Login nastaven!");
            await AuthService.SetupAutoLoginAsync(request);
            NavigationManager.NavigateTo("/orders");
            return;
        }

        await AuthService.SetSessionAsync(loginResult.AsT0);
        NavigationManager.NavigateTo("/orders");

        // No need to reset isLoggingIn anymore..
    }

    private async Task<bool> ValidateModel()
    {
        if (string.IsNullOrWhiteSpace(ModelUsername))
        {
            await ToastService.ShowErrorAsync("Vypl�te u�ivatelsk� jm�no");
            return false;
        }

        if (string.IsNullOrWhiteSpace(ModelPassword))
        {
            await ToastService.ShowErrorAsync("Vypl�te heslo");
            return false;
        }

        return true;
    }
}