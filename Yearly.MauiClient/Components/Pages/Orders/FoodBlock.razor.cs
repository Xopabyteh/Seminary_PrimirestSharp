using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Common;
using Yearly.MauiClient.Services.SharpApiFacades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class FoodBlock
{
    /// <summary>
    /// If null, the item is not ordered
    /// </summary>
    [Parameter] public PrimirestOrderIdentifierDTO? OrderIdentifier { get; set; }
    private bool isOrdered => OrderIdentifier is not null;

    [Parameter] public FoodDTO Food { get; set; } = null!;
    /// <summary>
    /// True when the food block knows about it's final state: that it is either ordered or unordered
    /// </summary>
    [Parameter] public bool IsLoaded { get; set; } = false;

    [Inject] private OrdersFacade OrdersFacade { get; set; } = null!;
    [Inject] private ToastService ToastService { get; set; } = null!;

    public async void OnOrderedClicked()
    {
        if (!isOrdered)
        {
            await NewOrder();
        }
        else
        {
            await CancelOrder();
        }
        StateHasChanged();
    }

    private async Task NewOrder()
    {
        var newOrderResult = await OrdersFacade.NewOrderAsync(Food.PrimirestFoodIdentifier);
        if (newOrderResult.IsT0)
        {
            var orderIdentifier = newOrderResult.AsT0;
            OrderIdentifier = orderIdentifier;
        } else if (newOrderResult.IsT1)
        {
            var problem = newOrderResult.AsT1;
            await ToastService.ShowErrorAsync(problem.Title);
        }
    }

    private async Task CancelOrder()
    {
        var response = await OrdersFacade.CancelOrderAsync(OrderIdentifier!);
        if (response is null)
        {
            //No error
            OrderIdentifier = null;
        }
        else
        {
            await ToastService.ShowErrorAsync(response.Value.Title);
        }
    }
}