using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Common;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class FoodBlock
{
    /// <summary>
    /// If null, the item is not ordered
    /// </summary>
    [Parameter] public PrimirestOrderIdentifierDTO? OrderIdentifier { get; set; }
    private bool isOrdered => OrderIdentifier is not null;

    [Parameter]
    public FoodDTO Food { get; set; } = null!;

    [Inject] private OrdersFacade OrdersFacade { get; set; } = null!;

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
        try
        {
            var newOrder = await OrdersFacade.NewOrderAsync(Food.PrimirestFoodIdentifier);
            OrderIdentifier = newOrder;
        }
        catch
        {
            //Todo:
        }
    }

    private async Task CancelOrder()
    {
        try
        {
            await OrdersFacade.CancelOrderAsync(OrderIdentifier!);
            OrderIdentifier = null;
        }
        catch
        {
            //Todo:
        }
    }
}