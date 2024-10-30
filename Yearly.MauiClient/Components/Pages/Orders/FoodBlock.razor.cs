using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.Contracts.Common;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class FoodBlock
{
    [Inject] private IJSRuntime _js { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    [Parameter] public FoodDTO Food { get; set; } = null!;
    [Parameter] public bool IsOrdered { get; set; }
    /// <summary>
    /// True when the food block knows about its final state: that it is either ordered or unordered
    /// </summary>
    [Parameter] public bool IsLoaded { get; set; } = false;
    [Parameter] public bool IsOrdering { get; set; } = false;
    protected bool isInteractable => IsLoaded && !IsOrdering;

    [Parameter] public EventCallback<FoodBlock> OnOrderClicked { get; set; }
    [Parameter] public decimal FoodPrice { get; set; }

    [Parameter] public bool ShowAddPhoto { get; set; }


    //<div class="images">
    private ElementReference imagesReference;
    //<div class="controls">
    private ElementReference imagesControlsReference;

    private async Task RaiseOnOrderClickedEvent()
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

        await _js.InvokeVoidAsync("FoodBlock.initializeImagesSlider", imagesReference, imagesControlsReference);
    }

    private void Handle_AddPhotoClicked(FoodDTO forFood)
    {
        _navigationManager.NavigateTo($"/photo?selectedFoodId={forFood.FoodId}");
    }
}