using Yearly.Contracts.Common;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Services;

/// <summary>
/// Lazy loads menus and orders when requested, caches them.
/// Call <see cref="NewOrderCreated"/> and <see cref="OrderCanceled"/> when an order is created or canceled
/// to invalidate the cache
/// </summary>
public class MenuAndOrderCacheService
{
    private readonly OrdersFacade _ordersFacade;
    private readonly MenusFacade _menusFacade;

    private List<WeeklyMenuDTO>? cachedAvailableMenus;
    //Due to primirest pagination:
    //Key: weekId, value: Orders
    private Dictionary<int, List<OrderDTO>> cachedOrdersForWeeks = new();

    public MenuAndOrderCacheService(OrdersFacade ordersFacade, MenusFacade menusFacade)
    {
        _ordersFacade = ordersFacade;
        _menusFacade = menusFacade;
    }

    public async Task<IReadOnlyList<WeeklyMenuDTO>> AvailableMenusCachedAsync()
    {
        if (cachedAvailableMenus is not null)
            return cachedAvailableMenus;

        //Cache
        var menus = await _menusFacade.GetAvailableMenusAsync();
        cachedAvailableMenus = menus.WeeklyMenus;

        return cachedAvailableMenus;
    }


    public async Task<IReadOnlyList<OrderDTO>> MyOrdersForWeekCachedAsync(int weekId)
    {
        if (cachedOrdersForWeeks.TryGetValue(weekId, out var cachedOrders))
            return cachedOrders;

        //Cache
        var ordersResponse = await _ordersFacade.GetMyOrdersForWeekAsync(weekId);
        lock (cachedOrdersForWeeks) //In case of a multi-threaded request
        {
            cachedOrdersForWeeks.Add(weekId, ordersResponse.Orders);
        }

        return ordersResponse.Orders;
    }

    public void NewOrderCreated(int forWeekId, OrderDTO newOrder)
    {
        //Add the new order
        cachedOrdersForWeeks[forWeekId].Add(newOrder);
    }

    public void OrderCanceled(int forWeekId, OrderDTO orderCanceled)
    {
        var ordersForWeek = cachedOrdersForWeeks[forWeekId];
        ordersForWeek.Remove(orderCanceled);
    }
}