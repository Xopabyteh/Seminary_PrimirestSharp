using Yearly.MauiClient.Components.Common;

namespace Yearly.MauiClient.Services.Toast;

public class ToastService
{
    public Task ShowErrorAsync(string message)
        => ToastManager.Instance!.ShowErrorAsync(message);

    public Task ShowSuccessAsync(string message)
        => ToastManager.Instance!.ShowSuccessAsync(message);

    /// <param name="message"></param>
    /// <param name="durationMillis">In milliseconds, -1 for infinite</param>
    /// <returns></returns>
    public Task ShowInformationAsync(string message, int durationMillis = 3000)
        => ToastManager.Instance!.ShowInformationAsync(message, durationMillis);
}