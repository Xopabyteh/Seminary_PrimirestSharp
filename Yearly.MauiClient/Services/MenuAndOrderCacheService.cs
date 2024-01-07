using System.Collections.Concurrent;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Services;

/// <summary>
/// Call <see cref="NewOrderCreated"/> and <see cref="OrderCanceled"/> when an order is created or canceled
/// to invalidate the cache
/// </summary>
public class MenuAndOrderCacheService
{
    private readonly OrdersFacade _ordersFacade;
    private readonly MenusFacade _menusFacade;

    private List<WeeklyMenuDTO> cachedAvailableMenus = new();
    private readonly TaskCompletionSource<IReadOnlyList<WeeklyMenuDTO>> _menusLoadedTcs = new();
    
    //Due to primirest pagination:
    //Key: weekId, value: Orders
    private ConcurrentDictionary<int, List<OrderDTO>> cachedOrdersForWeeks = new();
    private readonly TaskCompletionSource<IReadOnlyDictionary<int, List<OrderDTO>>> _ordersLoadedTcs = new();

    public decimal Balance { get; private set; }
    private readonly TaskCompletionSource<bool> _balanceLoadedTcs = new();

    public decimal OrderedFor { get; private set; }
    private readonly TaskCompletionSource<bool> _orderedForLoadedTcs = new();
    public event Action? OnOrderedForChanged;

    public Task<IReadOnlyList<WeeklyMenuDTO>> CachedMenusAsync() => _menusLoadedTcs.Task;
    public Task<IReadOnlyDictionary<int, List<OrderDTO>>> CachedOrdersForWeeksAsync() => _ordersLoadedTcs.Task;
    //public Task<decimal> CachedBalanceAsync() => _balanceLoadedTcs.Task;
    //public Task<decimal> CachedOrderedForAsync() => _orderedForLoadedTcs.Task;

    public Task WaitUntilBalanceLoaded() => _balanceLoadedTcs.Task;
    public Task WaitUntilOrderedForLoaded() => _orderedForLoadedTcs.Task;

    public MenuAndOrderCacheService(OrdersFacade ordersFacade, MenusFacade menusFacade)
    {
        _ordersFacade = ordersFacade;
        _menusFacade = menusFacade;
        //Todo: somehow LoadIntoCache on login
        //_authService = authService;
        //_authService.OnLogin += _authService_OnLogin;
    }

    //private void _authService_OnLogin()
    //{
    //    //Load menus and orders into cache
    //    Task.Run(LoadIntoCacheAsync);
    //}

    /// <summary>
    /// Loads data into both <see cref="cachedAvailableMenus"/> and <see cref="cachedOrdersForWeeks"/>
    /// from the data from the facade. Requires the client to be authenticated.
    /// </summary>
    /// <returns></returns>
    public async Task LoadIntoCacheAsync()
    {
        // AvailableMenus
        var menuResponse = await _menusFacade.GetAvailableMenusAsync();
        cachedAvailableMenus = menuResponse.WeeklyMenus;
        _menusLoadedTcs.SetResult(cachedAvailableMenus.AsReadOnly());

        //Orders
        var fillOrdersTasks = cachedAvailableMenus.Select(m => Task.Run(async () =>
        {
            var ordersForWeekResponse = await _ordersFacade.GetMyOrdersForWeekAsync(m.PrimirestMenuId);
            cachedOrdersForWeeks.TryAdd(m.PrimirestMenuId, ordersForWeekResponse.Orders);
        }));

        await Task.WhenAll(fillOrdersTasks);
        _ordersLoadedTcs.SetResult(cachedOrdersForWeeks.AsReadOnly());

        await LoadBalanceAsync();
        ReCalculateOrderedFor();
        _orderedForLoadedTcs.SetResult(true); //Signal that orders are loaded
    }

    public void NewOrderCreated(int forWeekId, OrderDTO newOrder)
    {
        //Add the new order
        cachedOrdersForWeeks[forWeekId].Add(newOrder);

        ReCalculateOrderedFor();
        OnOrderedForChanged?.Invoke();
    }

    public void OrderCanceled(int forWeekId, OrderDTO orderCanceled)
    {
        //Remove order
        var ordersForWeek = cachedOrdersForWeeks[forWeekId];
        ordersForWeek.Remove(orderCanceled);

        ReCalculateOrderedFor();
        OnOrderedForChanged?.Invoke();
    }

    private void ReCalculateOrderedFor()
    {
        //Calcuate "Objednáno za" based on our orders
        //Check PSharp Docs section `Balance calculation`

        OrderedFor = cachedOrdersForWeeks.Values
            .SelectMany(orders => orders)
            .Sum(o => o.PrimirestOrderData.PriceCzechCrowns);
    }

    private async Task LoadBalanceAsync()
    {
        //Load "Stav konta"
        Balance = await _ordersFacade.GetMyBalanceWithoutOrdersAccounted();
        _balanceLoadedTcs.SetResult(true); //Signal that balance is loaded
    }
}