using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Common;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class DailyMenu
{
    [Parameter] public DailyMenuDTO DailyMenuDTO { get; set; } = null!;

    [Parameter] public WeeklyMenu Parent { get; set; } = null!;
    [Parameter] public IReadOnlyList<OrderDTO> WeekOrders { get; set; } = null!;
    [Parameter] public bool WeekOrdersLoaded { get; set; } = false;

    [Inject] private OrdersFacade OrdersFacade { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;
    [Inject] private ToastService ToastService { get; set; } = null!;

    private OrderDTO? order;

    protected override async Task OnParametersSetAsync()
    {
        order = WeekOrders.FirstOrDefault(o => DailyMenuDTO.Foods.Any(f => f.FoodId == o.SharpFoodId));
    }

    private async void HandleFoodOnOrderClicked(FoodBlock obj)
    {
        if (order is not null)
        {
            //We have something ordered

            if (order.SharpFoodId == obj.Food.FoodId)
            {
                //We clicked on the same food we already have ordered
                //Cancel order

                var didCancel = await CancelOrder(order.PrimirestOrderData);
                if (!didCancel)
                    return; //Error

                MenuAndOrderCacheService.OrderCanceled(Parent.WeeklyMenuDTO.PrimirestMenuId, order);
                order = null;
            }
            else
            {
                //We clicked on a new food, create new order and cancel old one

                var newOrderIdentifier = await NewOrderAsync(obj.Food);

                if (newOrderIdentifier is null)
                    return; //error, return

                //Cancel old food from cache
                MenuAndOrderCacheService.OrderCanceled(Parent.WeeklyMenuDTO.PrimirestMenuId, order);

                //Set new order
                order = new OrderDTO(obj.Food.FoodId, newOrderIdentifier);

                //Add new food to cache
                MenuAndOrderCacheService.NewOrderCreated(Parent.WeeklyMenuDTO.PrimirestMenuId, order);
            }
        }
        else
        {
            //We don't have anything ordered yet, ONLY create a new order
            var newOrderIdentifier = await NewOrderAsync(obj.Food);

            if (newOrderIdentifier is null)
                return; //error, return

            //Set new order
            order = new OrderDTO(obj.Food.FoodId, newOrderIdentifier);

            //Add new food to cache
            MenuAndOrderCacheService.NewOrderCreated(Parent.WeeklyMenuDTO.PrimirestMenuId, order);
        }

        StateHasChanged();
    }

    ///<returns>The new order identifier if success, null if error</returns>
    private async Task<PrimirestOrderDataDTO?> NewOrderAsync(FoodDTO foodToOrder)
    {
        var newOrderResult = await OrdersFacade.NewOrderAsync(foodToOrder.PrimirestFoodIdentifier);
        if (newOrderResult.IsT0)
        {
            //Success

            var newOrderIdentifier = newOrderResult.AsT0;
            return newOrderIdentifier;
        }
        else if (newOrderResult.IsT1)
        {
            //Error

            var problem = newOrderResult.AsT1;
            await ToastService.ShowErrorAsync(problem.Title);
            return null;
        }

        throw new InvalidOperationException("Unexpected result");
    }

    /// <returns>True if canceled successfully, False if error</returns>
    private async Task<bool> CancelOrder(PrimirestOrderDataDTO orderIdentifier)
    {
        var response = await OrdersFacade.CancelOrderAsync(orderIdentifier);
        if (response is null)
        {
            //No error
            return true;
        }
        else
        {
            await ToastService.ShowErrorAsync(response.Value.Title);
            return false;
        }
    }
}