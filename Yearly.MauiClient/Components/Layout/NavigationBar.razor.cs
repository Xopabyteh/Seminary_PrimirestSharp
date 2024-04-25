using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi.Facades;

namespace Yearly.MauiClient.Components.Layout;

public partial class NavigationBar : IDisposable
{
    [Inject] protected MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] protected OrdersFacade _ordersFacade { get; set; } = null!;

    private bool isBalanceLoaded = false;
    private decimal balance = 0;
    private decimal orderedFor = 0;

    protected override void OnInitialized()
    {
        _menuAndOrderCacheService.OnOrderedForChanged += ReloadBalance;
    }

    private void ReloadBalance()
    {
        LoadBalance();
        StateHasChanged();
    }

    private void LoadBalance()
    {
        var balanceDetails = _menuAndOrderCacheService.GetBalanceDetails();
        balance = balanceDetails.BalanceCrowns;
        orderedFor = balanceDetails.OrderedForCrowns;
        isBalanceLoaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await _menuAndOrderCacheService.EnsureBalanceLoadedAsync();
        LoadBalance();

        StateHasChanged();
    }

    public void Dispose()
    {
        _menuAndOrderCacheService.OnOrderedForChanged -= ReloadBalance;
    }
}