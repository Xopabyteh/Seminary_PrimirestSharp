using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Dev;

public partial class DevPage
{
    

    protected override void OnInitialized()
    {
        UpdateOrderCheckerFSState();
    }

    #region Auth

    [Inject] private AuthService AuthService { get; set; } = null!;

    private Task RemoveSession()
    {
        return AuthService.RemoveSessionAsync();
    }

    #endregion

    #region OrderChecker

    private string orderCheckerFSState = "";

    private void StartOrderCheckerFS()
    {
#if ANDROID
        MainActivity.Instance.StartOrderCheckerBackgroundService();
        UpdateOrderCheckerFSState();
#endif
    }
    private void StopOrderCheckerFS()
    {
#if ANDROID
        MainActivity.Instance.StopOrderCheckerBackgroundService();
        UpdateOrderCheckerFSState();
#endif
    }

#if ANDROID
    private void UpdateOrderCheckerFSState()
    {
        orderCheckerFSState = OrderCheckerBackgroundService.IsRunning ? "Running" : "Stopped";
        StateHasChanged();
    }
#endif

    #endregion


}