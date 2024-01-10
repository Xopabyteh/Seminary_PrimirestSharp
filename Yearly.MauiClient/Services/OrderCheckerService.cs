using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Services;

public class OrderCheckerService
{
    private readonly MenusFacade _menusFacade;
    private readonly OrdersFacade _ordersFacade;

    public OrderCheckerService(MenusFacade menusFacade, OrdersFacade ordersFacade)
    {
        _menusFacade = menusFacade;
        _ordersFacade = ordersFacade;
    }

    public async Task<List<DailyMenuDTO>> GetDaysWithoutOrder()
    {
        List<DailyMenuDTO> daysWithoutOrder = new(2);
        var toCheck = await GetMenusToCheckAsync();
        foreach (var dMenuToCheck in toCheck.DMenusToCheck)
        {
            var doesDmHaveOrder =
                toCheck.OrdersAcrossMenus.Any(o => dMenuToCheck.Foods.Any(f => f.FoodId == o.SharpFoodId));

            if (doesDmHaveOrder)
                continue;

            daysWithoutOrder.Add(dMenuToCheck);
        }

        return daysWithoutOrder;
    }

    private async Task<(List<DailyMenuDTO> DMenusToCheck, List<OrderDTO> OrdersAcrossMenus)> GetMenusToCheckAsync()
    {
        (List<DailyMenuDTO> DMenusToCheck, List<OrderDTO> OrdersAcrossMenus) GetEmpty() =>
            (new(0), new(0));

        async Task<(List<DailyMenuDTO> DMenusToCheck, List<OrderDTO> OrdersAcrossMenus)> UseSharedMenuStrategy(
            DateTime tomorrow1Day,
            DateTime tomorrow2Day,
            AvailableMenusResponse menusResult)
        {


            //Get orders only from one menu
            var sharedMenu = menusResult.WeeklyMenus
                .FirstOrDefault(w =>
                    w.DailyMenus.Any(d => d.Date == tomorrow1Day));

            if (sharedMenu == default)
                return GetEmpty(); //Should not happen, because the one menu we have should always be this one, but let's be sure

            var ordersForSharedMenuResult = await _ordersFacade.GetMyOrdersForWeekAsync(sharedMenu.PrimirestMenuId);

            var tomorrow1DayDm = sharedMenu.DailyMenus
                .FirstOrDefault(d => d.Date == tomorrow1Day);
            var tomorrow2DayDm = sharedMenu.DailyMenus
                .FirstOrDefault(d => d.Date == tomorrow2Day);

            var dailyMenusToCheck = new List<DailyMenuDTO>(2);
            if (tomorrow1DayDm is not null)
            {
                dailyMenusToCheck.Add(tomorrow1DayDm);
            }
            if (tomorrow2DayDm is not null)
            {
                dailyMenusToCheck.Add(tomorrow2DayDm);
            }

            return (dailyMenusToCheck, ordersForSharedMenuResult.Orders);
        }

        async Task<(List<DailyMenuDTO> DMenusToCheck, List<OrderDTO> OrdersAcrossMenus)> UseSeparateMenuStrategy(
            DateTime tomorrow1Day,
            DateTime tomorrow2Day,
            AvailableMenusResponse menusResult)
        {
            //Get orders for two menus
            var firstMenu = menusResult.WeeklyMenus
                .FirstOrDefault(w =>
                    w.DailyMenus.Any(d => d.Date == tomorrow1Day));

            var secondMenu = menusResult.WeeklyMenus
                .FirstOrDefault(w =>
                    w.DailyMenus.Any(d => d.Date == tomorrow2Day));

            var dailyMenusToCheck = new List<DailyMenuDTO>(2);
            var ordersAcrossMenus = new List<OrderDTO>(14);

            if (firstMenu != default)
            {
                var tomorrow1DayDm = firstMenu.DailyMenus
                    .FirstOrDefault(d => d.Date == tomorrow1Day);

                if (tomorrow1DayDm is not null)
                {
                    var ordersForMenu = await _ordersFacade.GetMyOrdersForWeekAsync(firstMenu.PrimirestMenuId);

                    dailyMenusToCheck.Add(tomorrow1DayDm);
                    ordersAcrossMenus.AddRange(ordersForMenu.Orders);
                }
            }

            if (secondMenu != default)
            {
                var tomorrow2DayDm = secondMenu.DailyMenus
                    .FirstOrDefault(d => d.Date == tomorrow2Day);

                if (tomorrow2DayDm is not null)
                {
                    var ordersForMenu = await _ordersFacade.GetMyOrdersForWeekAsync(secondMenu.PrimirestMenuId);

                    dailyMenusToCheck.Add(tomorrow2DayDm);
                    ordersAcrossMenus.AddRange(ordersForMenu.Orders);
                }
            }


            return (dailyMenusToCheck, ordersAcrossMenus);
        }

        var menusResult = await _menusFacade.GetAvailableMenusAsync();

        if (menusResult.WeeklyMenus.Count == 0)
            return GetEmpty(); // No menus


        var today = DateTime.Today;

        //Easy case: (N = now, - no action, # check day)
        //M T W T F S W
        //- N # # - - -

        //M T W T F S W
        //- - N # # - -

        //Harder case:
        //M T W T F S W
        //# # - - N - -

        //Or:
        //M T W T F S W
        //# # - - - N -

        //-> If today in <Monday; Wednesday>, check 2 days ahead as normal
        // Otherwise, if == Thursday: check Friday and Monday(next menu)
        // Otherwise, if > Thursday: check Monday and Tuesday(next menu)

        //If Thursday: use separate menu strategy
        //Else, use shared menu strategy

        //Today in <Monday; Wednesday>
        if (today.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Wednesday)
        {
            var tomorrow1Day = today.AddDays(1);
            var tomorrow2Day = today.AddDays(2);

            return await UseSharedMenuStrategy(tomorrow1Day, tomorrow2Day, menusResult);
        }
        //Today == Thursday
        if (today.DayOfWeek == DayOfWeek.Thursday)
        {
            var tomorrow1Day = today.AddDays(1); //Friday
            var tomorrow2Day = today.AddDays(4); //Monday (next menu)

            return await UseSeparateMenuStrategy(tomorrow1Day, tomorrow2Day, menusResult);
        }
        //Today in <Friday; Sunday>
        else
        {
            //Give monday, tuesday next menu
            var tomorrow1Day = today.AddDays(0 + ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7);
            var tomorrow2Day = today.AddDays(1 + ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7);

            return await UseSharedMenuStrategy(tomorrow1Day, tomorrow2Day, menusResult);
        }
    }
}