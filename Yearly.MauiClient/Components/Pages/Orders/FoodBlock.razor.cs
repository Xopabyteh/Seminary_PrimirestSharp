using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Common;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class FoodBlock
{
    [Parameter] public FoodDTO Food { get; set; } = null!;
    [Parameter] public bool IsOrdered { get; set; }
    /// <summary>
    /// True when the food block knows about it's final state: that it is either ordered or unordered
    /// </summary>
    [Parameter] public bool IsLoaded { get; set; } = false;

    [Parameter] public EventCallback<FoodBlock> OnOrderClicked { get; set; }

    private async void RaiseOnOrderClickedEvent()
    {
        await OnOrderClicked.InvokeAsync(this);
    }
}