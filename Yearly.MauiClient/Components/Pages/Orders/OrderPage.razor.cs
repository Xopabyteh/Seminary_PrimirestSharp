using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Animations;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class OrderPage
{
    [Inject] private MenusFacade MenusFacade { get; set; } = null!;
    [Inject] private OrdersFacade OrdersFacade { get; set; } = null!;


    private List<WeeklyMenuDTO> weeklyMenus = new();
    private List<OrderDTO> myOrders = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        //Get weekly menus
        var weeklyMenusResponse = await MenusFacade.GetAvailableMenusAsync();
        weeklyMenus = weeklyMenusResponse.WeeklyMenus;
        StateHasChanged();

        //Get orders
        var getOrdersTasks = weeklyMenus.Select(async weeklyMenu =>
        {
            var ordersForWeek = await OrdersFacade.GetMyOrdersForWeekAsync(weeklyMenu.PrimirestMenuId);
            lock (myOrders)
            {
                myOrders.AddRange(ordersForWeek.Orders);
            }
        });

        await Task.WhenAll(getOrdersTasks);
        StateHasChanged();
    }
}