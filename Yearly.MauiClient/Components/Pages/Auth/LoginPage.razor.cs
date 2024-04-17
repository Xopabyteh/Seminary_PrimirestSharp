using Microsoft.AspNetCore.Components;
using Shiny.Jobs;
using Shiny.Push;
using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Auth;

public partial class LoginPage
{
    [Inject] private AuthenticationFacade AuthenticationFacade { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ToastService ToastService { get; set; } = null!;

    [Inject] private IPushManager PushManager { get; set; } = null!;
    [Inject] private IJobManager JobManager { get; set; } = null!;

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
        await AuthService.EnsureAutoLoginStateLoadedAsync();

        //Are we setting up autologin?
        var pageUri = NavigationManager.Uri;
        isSettingUpAutoLogin = pageUri.Contains("setupAutoLogin");

        if (isSettingUpAutoLogin)
        {
            autoLoginChecked = true; //Check the auto login box
            await ToastService.ShowInformationAsync("Pøihlaš se znovu pro nastavení Auto Loginu",  durationMillis: -1);
            return; //We don't want to try to autologin,
                    // but wait for user typed login, so we can setup Auto Login
        }

        //1. Try get stored credentials
        if (AuthService.AutoLoginStoredCredentials is not null)
        {
            autoLoginChecked = true;
            isLoggingIn = true;
            StateHasChanged();

            //Try to Auto Login
            var loginResult = await AuthenticationFacade.LoginAsync(AuthService.AutoLoginStoredCredentials);
            if (loginResult.IsT1)
            {
                //Problem
                isLoggingIn = false;
                StateHasChanged();

                await ToastService.ShowErrorAsync("Auto login se nezdaøil, mìnil/a jste si heslo?");
                return;
            }
            // -> Successful login

            AuthService.SetSession(loginResult.AsT0);
            NavigationManager.NavigateTo("/orders");
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

        if (autoLoginChecked)
        {
            //We want to setup auto login with these credentials
            await AuthService.SetupAutoLoginAsync(request);
        }

        AuthService.SetSession(loginResult.AsT0);
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

    /// <summary>
    /// This is also the entry point to the application, try to request needed permissions
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        var jobAccess = await JobManager.RequestAccess();

        try
        {
            var pushAccess = await PushManager.RequestAccess();
        }
        catch (Exception e)
        {
            // Hub is unavailable
            // NOOP
        }
    }
}