using Microsoft.AspNetCore.Components;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification;
using Shiny;
#if ANDROID || IOS
using Shiny.Push;
using Yearly.Contracts.Menu;

#endif
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Dev;

public partial class DevPage
{
    protected override async Task OnInitializedAsync()
    {
    }

    protected override void OnInitialized()
    {
#if ANDROID || IOS
        notificationsRegistrationToken = _pushManager.RegistrationToken;
#endif
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
            async () => await MainActivity.Instance.TryStartOrderCheckerAsync(orderCheckerStartDelayMillis));
#endif
    }
    private void StopOrderChecker()
    {
#if ANDROID
        MainActivity.Instance.StopOrderChecker();
#endif
    }

    private void UpdateStatusOrderChecker()
    {
#if ANDROID
        var status = MainActivity.Instance.GetIsOrderCheckerBackgroundWorkerScheduled();
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