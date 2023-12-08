using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages;

public partial class IndexPage
{
    [Inject]
    public AuthService AuthService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var hasSession = await AuthService.TryLoadSessionAsync();
        if (hasSession)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        NavigationManager.NavigateTo("/orders");
    }
}