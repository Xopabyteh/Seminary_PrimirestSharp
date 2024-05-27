using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class WeeklyMenu
{
    [Parameter] public required WeeklyMenuDTO WeeklyMenuDTO { get; set; }
    private WeeklyMenuDTO? previousWeeklyMenuDTO; // Used to check if this parameter changed
    [Parameter] public required DailyMenuDTO? SelectedDailyMenu { get; set; }

    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private IJSRuntime _js { get; set; } = null!;

    private ElementReference selectedDailyMenuSection; //@ref
    
    private IReadOnlyList<OrderDTO> orders = Array.Empty<OrderDTO>();
    private bool ordersLoaded;

    protected override async Task OnParametersSetAsync()
    {
        if(WeeklyMenuDTO == previousWeeklyMenuDTO)
            return;

        previousWeeklyMenuDTO = WeeklyMenuDTO;
        ordersLoaded = false;
        StateHasChanged();

        // Get orders of this menu
        await _menuAndOrderCacheService.EnsureOrdersLoadedAsync(WeeklyMenuDTO.PrimirestMenuId);
        orders = _menuAndOrderCacheService.GetOrdersForWeek(WeeklyMenuDTO.PrimirestMenuId);
        ordersLoaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (SelectedDailyMenu is not null)
        {
            // Scroll to selected daily menu
            await _js.InvokeVoidAsync("WeeklyMenu.scrollToDailyMenu", selectedDailyMenuSection);
        }

        StateHasChanged();
    }
}