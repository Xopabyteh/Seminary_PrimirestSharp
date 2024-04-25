using System.Collections.Concurrent;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services.SharpApi.Facades;

namespace Yearly.MauiClient.Services;

/// <summary>
/// Call <see cref="NewOrderCreated"/> and <see cref="OrderCanceled"/> when an order is created or canceled
/// to invalidate the cache
/// </summary>
public class MenuAndOrderCacheService
{
    private readonly OrdersFacade _ordersFacade;
    private readonly MenusFacade _menusFacade;

    //Due to primirest pagination:
    //Key: weekId, value: Orders
    private ConcurrentDictionary<int, List<OrderDTO>> cachedOrdersForWeeks = new();
    private List<WeeklyMenuDTO>? cachedAvailableMenus;
    private MyBalanceResponse? cachedBalance;

    public event Action? OnOrderedForChanged;

    public MenuAndOrderCacheService(OrdersFacade ordersFacade, MenusFacade menusFacade)
    {
        _ordersFacade = ordersFacade;
        _menusFacade = menusFacade;
    }

    public void NewOrderCreated(int forWeekId, OrderDTO newOrder)
    {
        // Add the new order
        cachedOrdersForWeeks![forWeekId].Add(newOrder);

        // Shift balance
        var newOrderedFor = cachedBalance!.Value.OrderedForCrowns + newOrder.PrimirestOrderData.PriceCzechCrowns;
        cachedBalance = cachedBalance!.Value with {OrderedForCrowns = newOrderedFor};

        OnOrderedForChanged?.Invoke();
    }

    public void OrderCanceled(int forWeekId, OrderDTO orderCanceled)
    {
        // Remove order
        var ordersForWeek = cachedOrdersForWeeks![forWeekId];
        ordersForWeek.Remove(orderCanceled);

        // Shift balance
        var newOrderedFor = cachedBalance!.Value.OrderedForCrowns - orderCanceled.PrimirestOrderData.PriceCzechCrowns;
        cachedBalance = cachedBalance!.Value with { OrderedForCrowns = newOrderedFor};

        OnOrderedForChanged?.Invoke();
    }

    public async Task EnsureBalanceLoadedAsync()
    {
        if (cachedBalance is not null)
            return;

        cachedBalance = await _ordersFacade.GetBalance();
    }
    
    public async Task EnsureMenusLoadedAsync()
    {
        if (cachedAvailableMenus is not null)
            return;

        var menuResponse = await _menusFacade.GetAvailableMenusAsync();
        cachedAvailableMenus = menuResponse.WeeklyMenus;
    }

    public async Task EnsureOrdersLoadedAsync(int forWeekId)
    {
        if (cachedOrdersForWeeks.ContainsKey(forWeekId))
            return;

        var ordersForWeekResponse = await _ordersFacade.GetMyOrdersForWeekAsync(forWeekId);
        cachedOrdersForWeeks.TryAdd(forWeekId, ordersForWeekResponse.Orders);
    }

    public IReadOnlyList<OrderDTO> GetOrdersForWeek(int forWeekId)
        => cachedOrdersForWeeks[forWeekId];

    public IReadOnlyList<WeeklyMenuDTO> GetAvailableMenus()
        => cachedAvailableMenus!;

    public MyBalanceResponse GetBalanceDetails()
        => cachedBalance!.Value;

    public void InvalidateCache()
    {
        cachedAvailableMenus = null;
        cachedOrdersForWeeks.Clear();
        cachedBalance = null;
    }
}