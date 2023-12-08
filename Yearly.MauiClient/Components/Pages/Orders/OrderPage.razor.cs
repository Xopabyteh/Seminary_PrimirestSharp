using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class OrderPage
{
    [Inject] public MenusFacade MenusFacade { get; set; } = null!;

    private List<WeeklyMenuDTO> weeklyMenus = new();

    protected override async Task OnInitializedAsync()
    {
        var response = await MenusFacade.GetAvailableMenusAsync();
        weeklyMenus = response.WeeklyMenus;
    }
}