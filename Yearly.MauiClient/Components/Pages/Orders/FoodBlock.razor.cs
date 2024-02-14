using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.Contracts.Common;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class FoodBlock
{
    [Inject] private IJSRuntime JS { get; set; } = null!;

    [Parameter] public FoodDTO Food { get; set; } = null!;
    [Parameter] public bool IsOrdered { get; set; }
    /// <summary>
    /// True when the food block knows about it's final state: that it is either ordered or unordered
    /// </summary>
    [Parameter] public bool IsLoaded { get; set; } = false;

    [Parameter] public EventCallback<FoodBlock> OnOrderClicked { get; set; }

    //<div class="images">
    private ElementReference imagesReference;
    //<div class="controls">
    private ElementReference imagesControlsReference;

    private string foodPrice = "53~55";
    private async void RaiseOnOrderClickedEvent()
    {
        await OnOrderClicked.InvokeAsync(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (Food.PhotoLinks.Count < 2)
            return;

        //var module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Pages/Orders/FoodBlock.razor.js");
        //await module.InvokeVoidAsync("initializeImagesSlider", imagesReference, imagesControlsReference);

        await JS.InvokeVoidAsync("FoodBlock.initializeImagesSlider", imagesReference, imagesControlsReference);
    }
}