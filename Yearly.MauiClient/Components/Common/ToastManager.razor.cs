using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Yearly.MauiClient.Components.Common;
public partial class ToastManager
{
    public static ToastManager? Instance;

    protected override void OnInitialized()
    {
        Instance = this;
    }

    [Inject] protected IJSRuntime _js { get; set; } = null!;

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
