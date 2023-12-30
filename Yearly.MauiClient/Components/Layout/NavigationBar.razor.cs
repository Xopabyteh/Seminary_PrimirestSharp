using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Components.Layout;

public partial class NavigationBar
{
    [Inject] protected MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;
    [Inject] protected OrdersFacade OrdersFacade { get; set; } = null!;

    private bool isBalanceLoaded = false;
    private decimal balance = 0;

    private bool isOrderedForLoaded = false;
    private decimal orderedFor = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        balance = await MenuAndOrderCacheService.CachedBalanceAsync();
        isBalanceLoaded = true;

        orderedFor = await MenuAndOrderCacheService.CachedOrderedForAsync();
        isOrderedForLoaded = true;

        StateHasChanged();
    }
}