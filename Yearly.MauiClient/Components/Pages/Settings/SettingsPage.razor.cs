using Microsoft.AspNetCore.Components;
using Shiny.Push;
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
    private async Task OnOrderCheckerToggle(bool isChecked)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
#if ANDROID
        if (isChecked)
        {
            //Try Enable
            var didStart = await MainActivity.Instance.TryStartOrderCheckerAsync();
            if (didStart)
            {
                isOrderCheckerEnabled = isChecked;
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
            isOrderCheckerEnabled = isChecked;
        }
#endif
        Preferences.Set(k_OrderCheckerPrefKey, isOrderCheckerEnabled);

        StateHasChanged();
    }

    private async Task Logout()
    {
        await _authService.LogoutAsync();
        
        _navigationManager.NavigateTo("/login");
    }

    private Task RemoveAutoLogin()
    {
        //Display alert
        //Todo:

        //Remove
        _authService.RemoveAutoLogin();

        //Refresh page
        _navigationManager.Refresh(true);

        return Task.CompletedTask;
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
            1 => "1 Sdílená fotka",
            < 5 => $"{myPhotos.TotalPhotoCount} Sdílené fotky",
            _ => $"{myPhotos.TotalPhotoCount} Sdílených fotek"
        };
    }

    #region HubNotifications

    private List<string> activeTags = new (3);

    private void SetNotificationTag(NotificationTagContract tag, bool shouldBeActive, bool isUserSpecific)
    {
        var tagValue = isUserSpecific
            ? NotificationTagContract.AssembleUserSpecificTag(tag, _authService.UserDetailsLazy.UserId)
            : tag.Value;
        
        if (shouldBeActive)
        {
            activeTags.Add(tagValue);
#if ANDROID || IOS
            _pushNotificationsManager.Tags?.AddTag(tagValue);
#endif
        }
        else
        {
            activeTags.Remove(tagValue);
#if ANDROID || IOS
            _pushNotificationsManager.Tags?.RemoveTag(tagValue);
#endif
        }

        StateHasChanged();
    }

    private bool IsLoadedTagActive(NotificationTagContract tag, bool isUserSpecific)
        => activeTags.Contains(
            isUserSpecific
                ? NotificationTagContract.AssembleUserSpecificTag(tag, _authService.UserDetailsLazy.UserId)
                : tag.Value);

    #endregion
}