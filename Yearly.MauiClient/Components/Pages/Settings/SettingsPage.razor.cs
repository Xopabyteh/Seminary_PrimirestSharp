using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Common;
#if ANDROID || IOS
using Shiny;
using Shiny.Push;
using Yearly.MauiClient.Services.Toast;
#endif
using Yearly.Contracts.Notifications;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Settings;

public partial class SettingsPage
{
    [Inject] private AuthService _authService { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private MyPhotosCacheService _myPhotosCacheService { get; set; } = null!;

#if ANDROID || IOS
    [Inject] private ToastService _toastService { get; set; } = null!;
    [Inject] private IPushManager _pushNotificationsManager { get; set; } = null!;
#endif

    private decimal balance = 0;
    private decimal orderedFor = 0;
    private bool isMoneyLoaded = false;

    private DataFragmentDTO<PhotoLinkDTO> firstMyPhotosFragment;
    private bool firstMyPhotosFragmentLoaded = false;

    private bool isOrderCheckerEnabled;
    private const string k_OrderCheckerPrefKey = "ordercheckerenabled";

    private bool isLoggingOut = false;

    protected override void OnInitialized()
    {
#if ANDROID || IOS
        var loadedTags = _pushNotificationsManager.TryGetTags();
        if (loadedTags is not null)
        {
            activeTags.AddRange(loadedTags);
        }
#endif
        isOrderCheckerEnabled = Preferences.Get(k_OrderCheckerPrefKey, false);
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
    private async Task OnOrderCheckerToggle(bool newState)
    {
#if ANDROID
        if (newState)
        {
            // Try Enable
            var didStart = await MainActivity.Instance.TryStartOrderCheckerAsync();
            if (didStart)
            {
                isOrderCheckerEnabled = true;
            }
            else
            {
                await _toastService.ShowErrorAsync("Funkci nelze zapnout bez opr�vn�n�");
                isOrderCheckerEnabled = false;
            }
        }
        else
        {
            // Disable
            MainActivity.Instance.StopOrderChecker();
            isOrderCheckerEnabled = newState;
        }
#endif
        Preferences.Set(k_OrderCheckerPrefKey, isOrderCheckerEnabled);
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
            1 => "1 Sd�len� fotka",
            < 5 => $"{firstMyPhotosFragment.TotalCount} Sd�len� fotky",
            _ => $"{firstMyPhotosFragment.TotalCount} Sd�len�ch fotek"
        };
    }

    #region HubNotifications

    private List<string> activeTags = new (3);

    private async Task SetNotificationTag(NotificationTagContract tag, bool shouldBeActive)
    {
#if ANDROID || IOS
        var pushAccess = await _pushNotificationsManager.RequestAccess();
        if (pushAccess.Status != AccessState.Available)
        {
            await _toastService.ShowErrorAsync("Mus�te povolit notifikace, aby je aplikace mohla zoobrazit.");
        }
#endif
        if (shouldBeActive)
        {
#if ANDROID || IOS
            await _pushNotificationsManager.Tags?.AddTag(tag.Value)!;
#endif
            activeTags.Add(tag.Value);
        }
        else
        {
#if ANDROID || IOS
            await _pushNotificationsManager.Tags?.RemoveTag(tag.Value)!;
#endif
            activeTags.Remove(tag.Value);
        }
    }

    private bool IsLoadedTagActive(NotificationTagContract tag)
        => activeTags.Contains(tag.Value);
#endregion
}