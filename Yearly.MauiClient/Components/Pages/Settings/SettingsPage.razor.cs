using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Common;
using Yearly.MauiClient.Services.Toast;
using Yearly.Contracts.Notifications;
using Yearly.MauiClient.Components.Layout;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Settings;

public partial class SettingsPage
{
    //public const string BalanceMinimumThresholdPrefKey = "balanceminimumthreshold";
    //public const int BalanceMinimumThresholdDefault = 700; // Todo: change

    [Inject] private AuthService _authService { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private MyPhotosCacheService _myPhotosCacheService { get; set; } = null!;

    [Inject] private ToastService _toastService { get; set; } = null!;

    private decimal balance = 0;
    private decimal orderedFor = 0;
    private bool isMoneyLoaded = false;

    private DataFragmentDTO<PhotoLinkDTO> firstMyPhotosFragment;
    private bool firstMyPhotosFragmentLoaded = false;

    private bool isOrderCheckerEnabled;
    private const string k_OrderCheckerEnabledPrefKey = "ordercheckerenabled";
    
    private bool isBalanceCheckerEnabled;
    private const string k_BalanceCheckerEnabledPrefKey = "balancecheckerenabled";

    private bool isLoggingOut = false;

    protected override void OnInitialized()
    {
        isOrderCheckerEnabled = Preferences.Get(k_OrderCheckerEnabledPrefKey, false);
        isBalanceCheckerEnabled = Preferences.Get(k_BalanceCheckerEnabledPrefKey, false);
    }

    protected override async Task OnInitializedAsync()
    {
        await _menuAndOrderCacheService.EnsureBalanceLoadedAsync();

        var balanceDetails = _menuAndOrderCacheService.GetBalanceDetails();
        balance = balanceDetails.BalanceCrowns;
        orderedFor = balanceDetails.OrderedForCrowns;

        isMoneyLoaded = true;
        StateHasChanged();

        firstMyPhotosFragment = await _myPhotosCacheService.GetMyPhotosCachedAsync(0);
        firstMyPhotosFragmentLoaded = true;
        StateHasChanged();
    }

    // ReSharper disable once UnusedParameter.Local
    private async Task<bool> OnOrderCheckerToggle(bool shouldEnable)
    {
#if ANDROID
        if (shouldEnable)
        {
            // Try Enable
            var didStart = await OrderCheckerBackgroundWorker.TryStart();
            if (didStart)
            {
                isOrderCheckerEnabled = true;
            }
            else
            {
                await _toastService.ShowErrorAsync("Funkci nelze zapnout bez oprávnìní");
                isOrderCheckerEnabled = false;
            }
        }
        else
        {
            // Disable
            OrderCheckerBackgroundWorker.StopOrderChecker();
            isOrderCheckerEnabled = shouldEnable;
        }
#endif

        Preferences.Set(k_OrderCheckerEnabledPrefKey, isOrderCheckerEnabled);
        
        return isOrderCheckerEnabled;
    }

    private async Task<bool> OnBalanceCheckerToggle(bool shouldEnable)
    {
#if ANDROID
        if (shouldEnable)
        {
            // Try Enable
            var didStart = await BalanceCheckerBackgroundWorker.TryStart();
            if (didStart)
            {
                isBalanceCheckerEnabled = true;
            }
            else
            {
                await _toastService.ShowErrorAsync("Funkci nelze zapnout bez oprávnìní");
                isBalanceCheckerEnabled = false;
            }
        }
        else
        {
            // Disable
            BalanceCheckerBackgroundWorker.Stop();
            isBalanceCheckerEnabled = shouldEnable;
        }
#endif

        Preferences.Set(k_BalanceCheckerEnabledPrefKey, isBalanceCheckerEnabled);
        
        return isBalanceCheckerEnabled;
    }

    private async Task Logout()
    {
        isLoggingOut = true;
        StateHasChanged();

        await _authService.LogoutAsync();
        
        _navigationManager.NavigateTo("/login");
    }

    private string PhotosTextWithGrammar()
    {
        return firstMyPhotosFragment.TotalCount switch
        {
            1 => "1 Sdílená fotka",
            < 5 => $"{firstMyPhotosFragment.TotalCount} Sdílené fotky",
            _ => $"{firstMyPhotosFragment.TotalCount} Sdílených fotek"
        };
    }

    #region HubNotifications

    private List<string> activeTags = new (3);

    /// <returns>New tag state. True is tag set, false is tag unset</returns>
    private async Task<bool> SetNotificationTag(NotificationTagContract tag, bool shouldBeActive)
    {
        await Task.CompletedTask;
        return true;
        //var pushAccess = await _pushNotificationsManager.RequestAccess();
        //if (pushAccess.Status != AccessState.Available)
        //{
        //    await _toastService.ShowErrorAsync("Musíte povolit notifikace, aby je aplikace mohla zoobrazit.");
        //    return false;
        //}

        //if (shouldBeActive)
        //{
        //    await _pushNotificationsManager.Tags?.AddTag(tag.Value)!;
        //    activeTags.Add(tag.Value);
        //    return true;
        //}

        //await _pushNotificationsManager.Tags?.RemoveTag(tag.Value)!;
        //activeTags.Remove(tag.Value);
        //return false;
    }

    private bool IsLoadedTagActive(NotificationTagContract tag)
        => activeTags.Contains(tag.Value);
#endregion

    private Task<bool> ToggleDarkMode(bool newState)
    {
        if (ThemeManager.Instance is null)
            return Task.FromResult(false);

        ThemeManager.Instance.IsDarkMode = newState;
        return Task.FromResult(newState);
    }
}