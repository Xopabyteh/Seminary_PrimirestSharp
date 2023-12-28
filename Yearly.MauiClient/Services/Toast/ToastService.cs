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
}