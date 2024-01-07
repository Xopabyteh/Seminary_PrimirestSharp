using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class WeeklyMenu
{
    [Parameter] public required WeeklyMenuDTO WeeklyMenuDTO { get; set; }

    [Inject] public MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;

    private IReadOnlyList<OrderDTO> orders = Array.Empty<OrderDTO>();
    private bool ordersLoaded;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        //Get orders of this menu
        var cachedOrdersForWeeks = await MenuAndOrderCacheService.CachedOrdersForWeeksAsync();
        orders = cachedOrdersForWeeks[WeeklyMenuDTO.PrimirestMenuId];
        ordersLoaded = true;
        StateHasChanged();
    }


}