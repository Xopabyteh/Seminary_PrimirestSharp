using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class WeeklyMenu
{
    [Parameter] public required WeeklyMenuDTO WeeklyMenuDTO { get; set; }
    [Parameter] public required DailyMenuDTO? SelectedDailyMenu { get; set; }

    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private IJSRuntime _js { get; set; } = null!;

    private ElementReference selectedDailyMenuSection; //@ref
    
    private IReadOnlyList<OrderDTO> orders = Array.Empty<OrderDTO>();
    private bool ordersLoaded;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (SelectedDailyMenu is not null)
        {
            //Scroll to selected daily menu
            await _js.InvokeVoidAsync("WeeklyMenu.scrollToDailyMenu", selectedDailyMenuSection);
        }

        //Get orders of this menu
        var cachedOrdersForWeeks = await _menuAndOrderCacheService.CachedOrdersForWeeksAsync();
        orders = cachedOrdersForWeeks[WeeklyMenuDTO.PrimirestMenuId];
        ordersLoaded = true;
        StateHasChanged();
    }
}