using Microsoft.AspNetCore.Components;
using Shiny;
using Shiny.Push;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Dev;

public partial class DevPage
{
    protected override void OnInitialized()
    {
        notificationsRegistrationToken = _pushManager.RegistrationToken;
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

    #region Notifications

    [Inject] private IPushManager _pushManager { get; set; } = null!;
    private string? notificationsRegistrationToken = null;
    private AccessState notificationsPushAccessState = AccessState.Unknown;

    private async Task Register()
    {
        var accessState = await _pushManager.RequestAccess();
        notificationsRegistrationToken = accessState.RegistrationToken;
        notificationsPushAccessState = accessState.Status;
        
        StateHasChanged();
    }

    private async Task Unregister()
    {
        await _pushManager.UnRegister();
        notificationsPushAccessState = AccessState.Disabled;
        
        StateHasChanged();
    }

    #endregion

}