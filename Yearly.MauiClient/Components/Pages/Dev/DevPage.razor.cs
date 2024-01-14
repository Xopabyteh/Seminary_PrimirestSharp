using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Dev;

public partial class DevPage
{
    protected override void OnInitialized()
    {
    }

    #region Auth

    [Inject] private AuthService AuthService { get; set; } = null!;

    private Task RemoveSession()
    {
        return AuthService.RemoveSessionAsync();
    }

    #endregion

    #region OrderChecker

    private string orderCheckerStatus = "No status loaded";
    private long orderCheckerStartDelayMillis = 0;
    private void StartOrderChecker()
    {
#if ANDROID
        MainActivity.Instance.TryStartOrderCheckerAsync(orderCheckerStartDelayMillis);
#endif
    }
    private void StopOrderChecker()
    {
#if ANDROID
        MainActivity.Instance.StopOrderChecker();
#endif
    }

    private async Task UpdateStatusOrderChecker()
    {
#if ANDROID
        var status = MainActivity.Instance.GetIsOrderCheckerBackgroundWorkerScheduled();
        orderCheckerStatus = status ? "Scheduled/Running" : "Not scheduled";
        await Task.CompletedTask;
#endif
        StateHasChanged();
    }

    #endregion


}