using Microsoft.AspNetCore.Components;
using Shiny;
#if ANDROID || IOS
using Shiny.Push;

#endif
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Dev;

public partial class DevPage
{
    protected override void OnInitialized()
    {
#if ANDROID || IOS
        notificationsRegistrationToken = _pushManager.RegistrationToken;
#endif

        UpdateStatusOrderChecker();
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
        Task.Run(
            async () => await OrderCheckerBackgroundWorker.TryStartOrderCheckerAsync(orderCheckerStartDelayMillis));
#endif
    }
    private void StopOrderChecker()
    {
#if ANDROID
        OrderCheckerBackgroundWorker.StopOrderChecker();
#endif
    }

    private void UpdateStatusOrderChecker()
    {
#if ANDROID
        var status = OrderCheckerBackgroundWorker.GetIsOrderCheckerBackgroundWorkerScheduled();
        orderCheckerStatus = status ? "Scheduled/Running" : "Not scheduled";
#endif
        StateHasChanged();
    }

    #endregion

#region Notifications

#if ANDROID || IOS
    [Inject] private IPushManager _pushManager { get; set; } = null!;
#endif
    private string? notificationsRegistrationToken = null;
    private AccessState notificationsPushAccessState = AccessState.Unknown;

    private async Task RegisterNotifications()
    {
#if ANDROID || IOS
        var accessState = await _pushManager.RequestAccess();
        notificationsRegistrationToken = accessState.RegistrationToken;
        notificationsPushAccessState = accessState.Status;
        
        StateHasChanged();
#endif
    }

    private async Task UnregisterNotifications()
    {
#if ANDROID || IOS
        await _pushManager.UnRegister();
        notificationsPushAccessState = AccessState.Disabled;
        
        StateHasChanged();
#endif
    }

#endregion

}