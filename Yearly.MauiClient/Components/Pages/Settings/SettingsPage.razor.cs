using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Settings;

public partial class SettingsPage
{
    [Inject] private AuthService AuthService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;

    private decimal balance = 0;
    private decimal orderedFor = 0;
    private bool isMoneyLoaded = false;

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        
        NavigationManager.NavigateTo("/login");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await MenuAndOrderCacheService.WaitUntilBalanceLoaded();
        await MenuAndOrderCacheService.WaitUntilOrderedForLoaded();

        balance = MenuAndOrderCacheService.Balance;
        orderedFor = MenuAndOrderCacheService.OrderedFor;
        isMoneyLoaded = true;
        StateHasChanged();
    }
}