using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Common;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class DailyMenu
{
    [Parameter] public DailyMenuDTO DailyMenuDTO { get; set; } = null!;

    [Parameter] public WeeklyMenu Parent { get; set; } = null!;
    [Parameter] public IReadOnlyList<OrderDTO> WeekOrders { get; set; } = null!;
    [Parameter] public bool WeekOrdersLoaded { get; set; } = false;

    [Inject] private OrdersFacade _ordersFacade { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private ToastService _toastService { get; set; } = null!;
    [Inject] protected AuthService _authService { get; set; } = null!;

    private OrderDTO? order;

    /// <summary>
    /// Foods that are currently involed in being ordered.
    /// Used by foodblocks to tell them whether they can be used now or not
    /// </summary>
    private HashSet<Guid> foodIdsInvolvedInOrdering = new(3);

    protected override void OnParametersSet()
    {
        order = WeekOrders.FirstOrDefault(o => DailyMenuDTO.Foods.Any(f => f.FoodId == o.SharpFoodId));
    }

    private async Task HandleFoodOnOrderClicked(FoodBlock obj)
    {
        if (order is not null)
        {
            //We have something ordered

            if (order.SharpFoodId == obj.Food.FoodId)
            {
                //We clicked on the same food we already have ordered
                //Cancel order

                foodIdsInvolvedInOrdering.Add(obj.Food.FoodId);
                StateHasChanged();

                var didCancel = await CancelOrder(order.PrimirestOrderData);
                if (!didCancel)
                {
                    //error, return
                    foodIdsInvolvedInOrdering.Clear();
                    StateHasChanged();
                    return; 
                }

                _menuAndOrderCacheService.OrderCanceled(Parent.WeeklyMenuDTO.PrimirestMenuId, order);
                order = null;
            }
            else
            {
                //We clicked on a new food, create new order and cancel old one

                foodIdsInvolvedInOrdering.Add(order.SharpFoodId);
                foodIdsInvolvedInOrdering.Add(obj.Food.FoodId);
                StateHasChanged();

                var newOrderIdentifier = await NewOrderAsync(obj.Food);

                if (newOrderIdentifier is null)
                {
                    //error, return
                    foodIdsInvolvedInOrdering.Clear();
                    StateHasChanged();
                    return; 
                }

                //Cancel old food from cache
                _menuAndOrderCacheService.OrderCanceled(Parent.WeeklyMenuDTO.PrimirestMenuId, order);

                //Set new order
                order = new OrderDTO(obj.Food.FoodId, newOrderIdentifier);

                //Add new food to cache
                _menuAndOrderCacheService.NewOrderCreated(Parent.WeeklyMenuDTO.PrimirestMenuId, order);
            }
        }
        else
        {
            //We don't have anything ordered yet, ONLY create a new order

            foodIdsInvolvedInOrdering.Add(obj.Food.FoodId);
            StateHasChanged();

            var newOrderIdentifier = await NewOrderAsync(obj.Food);

            if (newOrderIdentifier is null)
            {
                //error, return
                foodIdsInvolvedInOrdering.Clear();
                StateHasChanged();
                return; 
            }

            //Set new order
            order = new OrderDTO(obj.Food.FoodId, newOrderIdentifier);

            //Add new food to cache
            _menuAndOrderCacheService.NewOrderCreated(Parent.WeeklyMenuDTO.PrimirestMenuId, order);
        }

        foodIdsInvolvedInOrdering.Clear();
        StateHasChanged();
    }

    ///<returns>The new order identifier if success, null if error</returns>
    private async Task<PrimirestOrderDataDTO?> NewOrderAsync(FoodDTO foodToOrder)
    {
        var newOrderResult = await _ordersFacade.NewOrderAsync(foodToOrder.PrimirestFoodIdentifier);
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
            await _toastService.ShowErrorAsync(problem.GetLocalizedMessage());
            return null;
        }

        throw new InvalidOperationException("Unexpected result");
    }

    /// <returns>True if canceled successfully, False if error</returns>
    private async Task<bool> CancelOrder(PrimirestOrderDataDTO orderData)
    {
        var response = await _ordersFacade.CancelOrderAsync(orderData.PrimirestOrderIdentifier);
        if (response is null)
        {
            //No error
            return true;
        }
        else
        {
            await _toastService.ShowErrorAsync(response.Value.GetLocalizedMessage());
            return false;
        }
    }

    /// <summary>
    /// When ordered, we know the price so use that one.
    /// Until ordered, we may only use the prediction...
    /// </summary>
    /// <param name="forFood"></param>
    /// <returns></returns>
    private decimal GetFoodPrice(FoodDTO forFood)
    {
        // If we have ordered this food, use the price we know
        if (order is not null && order.SharpFoodId == forFood.FoodId)
            return order.PrimirestOrderData.PriceCzechCrowns;

        // If we haven't ordered this food, use the prediction
        return _authService.ActiveUserDetailsLazy.PredictedPriceCzechCrowns;
    }
}