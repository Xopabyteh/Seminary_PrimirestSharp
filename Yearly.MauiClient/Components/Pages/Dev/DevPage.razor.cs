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

    private void StartOrderCheckerFS()
    {
#if ANDROID
        MainActivity.Instance.StartOrderCheckerBackgroundWorker();
#endif
    }
    private void StopOrderCheckerFS()
    {
#if ANDROID
        MainActivity.Instance.StopOrderCheckerBackgroundWorker();
#endif
    }


    #endregion


}