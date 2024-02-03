using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Yearly.MauiClient.Components.Pages.WebRequestProblem;

public partial class NoInternetAccessPage
{
    [Inject] private IJSRuntime _js { get; set; } = null!;

    private async Task Retry()
    {
        await _js.InvokeVoidAsync("history.back");
    }
}