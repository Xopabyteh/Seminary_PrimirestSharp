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
        UpdateStatusBalanceChecker();
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
            async () => await OrderCheckerBackgroundWorker.TryStart(orderCheckerStartDelayMillis));
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
        var status = MainActivity.Instance.GetIsBackgroundWorkerScheduled(OrderCheckerBackgroundWorker.WorkNameTag);
        orderCheckerStatus = status ? "Scheduled/Running" : "Not scheduled";
#endif
        StateHasChanged();
    }

    #endregion

    #region BalanceChecker

    private string balanceCheckerStatus = "No status loaded";
    private long balanceCheckerStartDelayMillis = 0;
    private void StartBalanceChecker()
    {
        #if ANDROID
        Task.Run(
            async () => await BalanceCheckerBackgroundWorker.TryStart(balanceCheckerStartDelayMillis));
        #endif
    }

    private void StopBalanceChecker()
    {
        #if ANDROID
        BalanceCheckerBackgroundWorker.Stop();
        #endif
    }

    private void UpdateStatusBalanceChecker()
    {
        #if ANDROID
        var status = MainActivity.Instance.GetIsBackgroundWorkerScheduled(BalanceCheckerBackgroundWorker.WorkNameTag);
        balanceCheckerStatus = status ? "Scheduled/Running" : "Not scheduled";
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