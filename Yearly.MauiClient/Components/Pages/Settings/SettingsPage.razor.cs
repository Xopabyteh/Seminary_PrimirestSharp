using Microsoft.AspNetCore.Components;
#if ANDROID || IOS
using Shiny;
using Shiny.Push;
using Yearly.MauiClient.Services.Toast;
#endif
using Yearly.Contracts.Notifications;
using Yearly.Contracts.Photos;
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

    private MyPhotosResponse myPhotos;
    private bool myPhotosLoaded = false;

    private bool isOrderCheckerEnabled;
    private const string k_OrderCheckerPrefKey = "ordercheckerenabled";

    protected override Task OnInitializedAsync()
    {
#if ANDROID || IOS
        var loadedTags = _pushNotificationsManager.TryGetTags();
        if (loadedTags is not null)
        {
            activeTags.AddRange(loadedTags);
        }
#endif
        isOrderCheckerEnabled = Preferences.Get(k_OrderCheckerPrefKey, false);

        return Task.CompletedTask;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    // ReSharper disable once UnusedParameter.Local
    private async Task OnOrderCheckerToggle(bool newState)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
#if ANDROID
        if (newState)
        {
            //Try Enable
            var didStart = await MainActivity.Instance.TryStartOrderCheckerAsync();
            if (didStart)
            {
                isOrderCheckerEnabled = newState;
            }
            else
            {
                //Todo: Show alert

                isOrderCheckerEnabled = false;
            }
        }
        else
        {
            //Disable
            MainActivity.Instance.StopOrderChecker();
            isOrderCheckerEnabled = newState;
        }
#endif
        Preferences.Set(k_OrderCheckerPrefKey, isOrderCheckerEnabled);
    }

    private async Task Logout()
    {
        await _authService.LogoutAsync();
        
        _navigationManager.NavigateTo("/login");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await _menuAndOrderCacheService.EnsureBalanceLoadedAsync();

        var balanceDetails = _menuAndOrderCacheService.GetBalanceDetails();
        balance = balanceDetails.BalanceCrowns;
        orderedFor = balanceDetails.OrderedForCrowns;

        isMoneyLoaded = true;
        StateHasChanged();

        myPhotos = await _myPhotosCacheService.GetMyPhotosCachedAsync();
        myPhotosLoaded = true;
        StateHasChanged();
    }

    private string PhotosTextWithGrammar()
    {
        return myPhotos.TotalPhotoCount switch
        {
            1 => "1 Sd�len� fotka",
            < 5 => $"{myPhotos.TotalPhotoCount} Sd�len� fotky",
            _ => $"{myPhotos.TotalPhotoCount} Sd�len�ch fotek"
        };
    }

    #region HubNotifications

    private List<string> activeTags = new (3);

    /// <returns>True if tag was set successfully</returns>
#pragma warning disable CS1998 // Only here due to windows dev build 
    private async Task SetNotificationTag(NotificationTagContract tag, bool shouldBeActive)
#pragma warning restore CS1998
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
            activeTags.Add(tag.Value);

            // Todo: move to background task
#if ANDROID || IOS
            await _pushNotificationsManager.Tags?.AddTag(tag.Value)!;
#endif
        }
        else
        {
            activeTags.Remove(tag.Value);

            // Todo: move to background task
#if ANDROID || IOS
            await _pushNotificationsManager.Tags?.RemoveTag(tag.Value)!;
#endif
        }

        StateHasChanged();
    }

    private bool IsLoadedTagActive(NotificationTagContract tag)
        => activeTags.Contains(tag.Value);
#endregion
}