using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Yearly.MauiClient.Services.SharpApi;

namespace Yearly.MauiClient.Components.Common;

/// <summary>
/// A global component "exception handling middleware".
/// Used to handle Connectivity and API availability exceptions.
/// Also catches other exceptions and displays an error message.
///
/// Only catches exceptions thrown within components. Exceptions from services are not caught.
/// </summary>
public class ComponentExceptionHandlerBase : ErrorBoundary
{
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private WebRequestProblemService _webRequestProblemService { get; set; } = null!;
    [Inject] private IJSRuntime _js { get; set; } = null!;
    protected bool DidProcessException { get; private set; } = false;
    protected override Task OnErrorAsync(Exception exception)
    {
        if (exception is HttpRequestException httpRequestException)
        {
            _webRequestProblemService.HttpRequestException = httpRequestException;
            _navigationManager.NavigateTo("/webRequestProblem", true);
            return Task.CompletedTask;
        }

        if (exception is SafeConnectionAwareHttpClientHandler.NoInternetAccessException)
        {
            _navigationManager.NavigateTo("/noInternet", true);
            return Task.CompletedTask;
        }

        DidProcessException = true;
        StateHasChanged();

#if DEBUG
        throw exception;
#endif

        return Task.CompletedTask;
    }

    protected async Task Retry()
    {
        await _js.InvokeVoidAsync("history.back");
    }
}