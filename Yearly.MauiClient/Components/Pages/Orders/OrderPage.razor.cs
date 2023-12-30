using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class OrderPage
{
    [Inject] private MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;

    private IReadOnlyList<WeeklyMenuDTO> weeklyMenus = Array.Empty<WeeklyMenuDTO>();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        //Get weekly menus
        weeklyMenus = await MenuAndOrderCacheService.CachedMenusAsync();
        StateHasChanged();
    }
}