using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Yearly.MauiClient.Components.Pages.WebRequestProblem;

public partial class NoInternetAccessPage
{
    [Inject] protected NavigationManager _navigationManager { get; set; } = null!;
    private async Task Retry()
    {
        _navigationManager.NavigateTo("/", true);
    }
}