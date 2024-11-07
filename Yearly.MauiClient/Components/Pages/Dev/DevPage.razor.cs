using Microsoft.AspNetCore.Components;
using Plugin.Firebase.CloudMessaging;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.Notifications;

namespace Yearly.MauiClient.Components.Pages.Dev;

public partial class DevPage
{
    protected override void OnInitialized()
    {
        notificationsRegistrationToken = Preferences.Get(PushNotificationHandlerService.k_FCMTokenPrefKey, null);

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

    private string? notificationsRegistrationToken = null;

    private async Task RegisterNotifications()
    {
        await CrossFirebaseCloudMessaging.Current.SubscribeToTopicAsync("debug_push");
    }

    private async Task UnregisterNotifications()
    {
        await CrossFirebaseCloudMessaging.Current.UnsubscribeFromTopicAsync("debug_push");
    }

#endregion

}