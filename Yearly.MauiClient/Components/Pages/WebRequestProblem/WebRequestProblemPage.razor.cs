using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.MauiClient.Services.SharpApi;

namespace Yearly.MauiClient.Components.Pages.WebRequestProblem;

public partial class WebRequestProblemPage
{
    [Inject] private WebRequestProblemService _webRequestProblemService { get; set; } = null!;
    [Inject] protected NavigationManager _navigationManager { get; set; } = null!;
    private async Task Retry()
    {
        _navigationManager.NavigateTo("/", true);
    }
}