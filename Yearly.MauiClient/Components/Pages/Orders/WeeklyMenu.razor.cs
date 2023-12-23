using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class WeeklyMenu
{
    [Parameter] public WeeklyMenuDTO WeeklyMenuDTO { get; set; }

    [Inject] public MenuAndOrderCacheService MenuAndOrderCacheService { get; set; }

    private IReadOnlyList<OrderDTO> orders = Array.Empty<OrderDTO>();
    private bool ordersLoaded;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        //Get orders of this menu
        orders = await MenuAndOrderCacheService.MyOrdersForWeekCachedAsync(WeeklyMenuDTO.PrimirestMenuId);
        ordersLoaded = true;
        StateHasChanged();
    }
}