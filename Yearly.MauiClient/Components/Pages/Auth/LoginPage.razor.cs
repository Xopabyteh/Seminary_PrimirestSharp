using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Auth;

public partial class LoginPage
{
    [Inject]
    private SharpAPIFacade SharpAPIFacade { get; set; } = null!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;


    [SupplyParameterFromForm]
    public string ModelUsername { get; set; } = string.Empty;

    [SupplyParameterFromForm]
    public string ModelPassword { get; set; } = string.Empty;

    private async void SubmitLogin()
    {
        var request = new LoginRequest(ModelUsername, ModelPassword);

        var loginResult = await SharpAPIFacade.LoginAsync(request);
        if (loginResult != default)
        {
            //NavigationManager.NavigateTo("/orders");
        }
    }
}