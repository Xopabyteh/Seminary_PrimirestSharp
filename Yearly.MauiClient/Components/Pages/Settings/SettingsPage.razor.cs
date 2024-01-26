using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Photos;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Settings;

public partial class SettingsPage
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;
    //[Inject] private PhotoFacade PhotoFacade { get; set; } = null!;
    [Inject] private MyPhotosCacheService MyPhotosCacheService { get; set; } = null!;

    private decimal balance = 0;
    private decimal orderedFor = 0;
    private bool isMoneyLoaded = false;

    private MyPhotosResponse myPhotos;
    private bool myPhotosLoaded = false;

    private bool isOrderCheckerEnabled;
    private const string k_OrderCheckerPrefKey = "ordercheckerenabled";

    protected override Task OnInitializedAsync()
    {
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
        await AuthService.LogoutAsync();
        
        NavigationManager.NavigateTo("/login");
    }

    private Task RemoveAutoLogin()
    {
        //Display alert
        //Todo:

        //Remove
        AuthService.RemoveAutoLogin();

        //Refresh page
        NavigationManager.Refresh(true);

        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await MenuAndOrderCacheService.WaitUntilBalanceLoaded();
        await MenuAndOrderCacheService.WaitUntilOrderedForLoaded();

        balance = MenuAndOrderCacheService.Balance;
        orderedFor = MenuAndOrderCacheService.OrderedFor;
        isMoneyLoaded = true;
        StateHasChanged();

        myPhotos = await MyPhotosCacheService.GetMyPhotosCachedAsync();
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
}