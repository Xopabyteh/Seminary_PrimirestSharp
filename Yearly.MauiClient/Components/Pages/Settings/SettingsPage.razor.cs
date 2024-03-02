using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Photos;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Settings;

public partial class SettingsPage
{
    [Inject] private AuthService _authService { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private IndependentNotificationHubService _notificationHubService { get; set; } = null!;
    [Inject] private MyPhotosCacheService _myPhotosCacheService { get; set; } = null!;

    private decimal balance = 0;
    private decimal orderedFor = 0;
    private bool isMoneyLoaded = false;

    private MyPhotosResponse myPhotos;
    private bool myPhotosLoaded = false;

    private bool isOrderCheckerEnabled;
    private const string k_OrderCheckerPrefKey = "ordercheckerenabled";

    protected override Task OnInitializedAsync()
    {
        LoadActiveHubNotificationTags();

        isOrderCheckerEnabled = Preferences.Get(k_OrderCheckerPrefKey, false);

        return Task.CompletedTask;
    }

    private async Task OnOrderCheckerToggle(bool isChecked)
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

        await _menuAndOrderCacheService.WaitUntilBalanceLoaded();
        await _menuAndOrderCacheService.WaitUntilOrderedForLoaded();

        balance = _menuAndOrderCacheService.Balance;
        orderedFor = _menuAndOrderCacheService.OrderedFor;
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

    string[] loadedNotificationTags; //These tags are not updated!, they are only loaded on app init.
    private bool isEnabled_PhaNewWaitingPhoto = false;
    private void LoadActiveHubNotificationTags()
    {
        loadedNotificationTags = _notificationHubService.GetTags();

        isEnabled_PhaNewWaitingPhoto = loadedNotificationTags.Contains("PhaNewWaitingPhoto");
    }

    private void OnPhaNewWaitingPhotoToggle(bool isChecked)
    {
        if (isChecked)
        {
            _notificationHubService.AddTag("PhaNewWaitingPhoto");
        }
        else
        {
            _notificationHubService.RemoveTag("PhaNewWaitingPhoto");
        }
    }

    #endregion
}