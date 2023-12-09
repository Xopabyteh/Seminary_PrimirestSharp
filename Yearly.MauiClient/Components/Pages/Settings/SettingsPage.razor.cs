using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Settings;

public partial class SettingsPage
{
    [Inject] public AuthService AuthService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        
        NavigationManager.NavigateTo("/login");
    }
}