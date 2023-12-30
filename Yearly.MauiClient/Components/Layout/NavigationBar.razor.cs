using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Components.Layout;

public partial class NavigationBar : IDisposable
{
    [Inject] protected MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;
    [Inject] protected OrdersFacade OrdersFacade { get; set; } = null!;

    private bool isBalanceLoaded = false;
    private decimal balance = 0;

    private bool isOrderedForLoaded = false;
    private decimal orderedFor = 0;

    protected override void OnInitialized()
    {
        MenuAndOrderCacheService.OnOrderedForChanged += ReloadOrderedFor;
    }

    private void ReloadOrderedFor()
    {
        orderedFor = MenuAndOrderCacheService.OrderedFor;
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await MenuAndOrderCacheService.WaitUntilBalanceLoaded();
        balance = MenuAndOrderCacheService.Balance;
        isBalanceLoaded = true;

        await MenuAndOrderCacheService.WaitUntilOrderedForLoaded();
        orderedFor = MenuAndOrderCacheService.OrderedFor;
        isOrderedForLoaded = true;

        StateHasChanged();
    }

    public void Dispose()
    {
        MenuAndOrderCacheService.OnOrderedForChanged -= ReloadOrderedFor;
    }
}