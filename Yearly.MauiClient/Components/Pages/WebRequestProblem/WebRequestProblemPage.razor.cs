using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.MauiClient.Services.SharpApi;

namespace Yearly.MauiClient.Components.Pages.WebRequestProblem;

public partial class WebRequestProblemPage
{
    [Inject] private WebRequestProblemService _webRequestProblemService { get; set; } = null!;
    [Inject] private IJSRuntime _js { get; set; } = null!;
    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    private async Task Retry()
    {
        await _js.InvokeVoidAsync("history.back");
    }
}