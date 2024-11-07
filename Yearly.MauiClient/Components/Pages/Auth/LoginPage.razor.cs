using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Auth;

public partial class LoginPage
{
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private AuthService _authService { get; set; } = null!;
    [Inject] private ToastService _toastService { get; set; } = null!;

    [SupplyParameterFromForm] public string ModelUsername { get; set; } = string.Empty;

    [SupplyParameterFromForm] public string ModelPassword { get; set; } = string.Empty;

    private bool isLoggingIn = false;
    private bool isSettingUpAutoLogin = false;
    private bool autoLoginChecked = false;

    /// <summary>
    /// Check client docs to better see what's happening here :)
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //Make sure auto login is loaded
        //(don't care if active, but loaded, so we know the state of it)
        await _authService.EnsureAutoLoginStateLoadedAsync();

        //Are we setting up autologin?
        var pageUri = _navigationManager.Uri;
        isSettingUpAutoLogin = pageUri.Contains("setupAutoLogin");

        if (isSettingUpAutoLogin)
        {
            autoLoginChecked = true; //Check the auto login box
            await _toastService.ShowInformationAsync("Pøihlaš se znovu pro nastavení Auto Loginu",  durationMillis: -1);
            return; //We don't want to try to autologin,
                    // but wait for user typed login, so we can setup Auto Login
        }

        //1. Try get stored credentials
        if (_authService.AutoLoginStoredCredentials is not null)
        {
            ModelUsername = _authService.AutoLoginStoredCredentials.Username;
            ModelPassword = "****";

            autoLoginChecked = true;
            isLoggingIn = true;
            StateHasChanged();

            //Try to Auto Login
            var loginResult = await _authService.AttemptAutoLoginAsync();
            if (loginResult is not null)
            {
                // -> Problem
                ModelPassword = string.Empty;
                isLoggingIn = false;
                StateHasChanged();

                await _toastService.ShowErrorAsync(loginResult.Value.GetLocalizedMessage());
                return;
            }

            // -> Successful login
            _navigationManager.NavigateTo("/orders");
        }

        //2. Try get credentials from user ...
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

        var loginResult = await _authService.LoginAsync(ModelUsername, ModelPassword);
        if (loginResult is not null)
        {
            // -> Problem
            isLoggingIn = false;
            StateHasChanged();

            await _toastService.ShowErrorAsync(loginResult.Value.GetLocalizedMessage());
            return;
        }

        // -> Successful login
        if (isSettingUpAutoLogin)
        {
            //We are setting up autologin, so we don't want to login the user, but just store the credentials
            await _toastService.ShowSuccessAsync("Auto Login nastaven!");
            await _authService.SetupAutoLoginAsync(ModelUsername, ModelPassword);
            _navigationManager.NavigateTo("/orders");
            return;
        }

        if (autoLoginChecked)
        {
            //We want to setup auto login with these credentials
            await _authService.SetupAutoLoginAsync(ModelUsername, ModelPassword);
        }

        _navigationManager.NavigateTo("/orders");

        // No need to reset isLoggingIn anymore...
    }

    private async Task<bool> ValidateModel()
    {
        if (string.IsNullOrWhiteSpace(ModelUsername))
        {
            await _toastService.ShowErrorAsync("Vyplòte uživatelské jméno");
            return false;
        }

        if (string.IsNullOrWhiteSpace(ModelPassword))
        {
            await _toastService.ShowErrorAsync("Vyplòte heslo");
            return false;
        }

        return true;
    }

    /// <summary>
    /// This is also the entry point to the application, try to request needed permissions
    /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task OnAfterRenderAsync(bool firstRender)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        if (!firstRender)
            // ReSharper disable once RedundantJumpStatement
            return;
    }
}