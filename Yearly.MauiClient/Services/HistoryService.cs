using Microsoft.JSInterop;

namespace Yearly.MauiClient.Services;

public class HistoryService
{
    private readonly IJSRuntime _js;
    
    public HistoryService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Attempts to go back in navigation history by 1
    /// unless that would put user into splash screen or smth
    /// </summary>
    /// <returns></returns>
    public async Task TryGoBackAsync()
    {
        await _js.InvokeVoidAsync("history.back");
    }
}