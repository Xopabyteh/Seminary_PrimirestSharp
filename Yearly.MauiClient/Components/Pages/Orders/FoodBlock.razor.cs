using Microsoft.AspNetCore.Components;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class FoodBlock
{
    [Parameter]
    public string FoodName { get; set; }

    [Parameter]
    public string Allergens { get; set; }

    [Parameter]
    public bool Ordered { get; set; } = false;

    [Parameter]
    public string ImageUrl { get; set; }

    private const string k_FoodPrice = "73Kè";


}