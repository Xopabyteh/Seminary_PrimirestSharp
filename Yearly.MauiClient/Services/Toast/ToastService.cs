using Microsoft.JSInterop;

namespace Yearly.MauiClient.Services.Toast;

public class ToastService
{
    private readonly IJSRuntime _js;

    public ToastService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task ShowErrorAsync(string message)
    {
        await _js.InvokeVoidAsync("toastError", message);
    }

    public async Task ShowSuccessAsync(string message)
    {
        await _js.InvokeVoidAsync("toastSuccess", message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="durationMillis">In milliseconds, -1 for infinite</param>
    /// <returns></returns>
    public async Task ShowInformationAsync(string message, int durationMillis = 3000)
    {
        await _js.InvokeVoidAsync("toastInformation", message, durationMillis);
    }
}