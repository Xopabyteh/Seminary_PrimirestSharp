using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Photos;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Components.Pages.Settings;

public partial class SettingsPage
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;
    [Inject] private PhotoFacade PhotoFacade { get; set; } = null!;

    private decimal balance = 0;
    private decimal orderedFor = 0;
    private bool isMoneyLoaded = false;

    private MyPhotosResponse myPhotos;
    private bool myPhotosLoaded = false;

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        
        NavigationManager.NavigateTo("/loginFromLogout");
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

        myPhotos = await PhotoFacade.GetMyPhotosAsync();
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