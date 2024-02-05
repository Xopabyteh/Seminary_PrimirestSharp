using Microsoft.AspNetCore.Components;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Photo;

public partial class PhotoPage
{
    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private PhotoFacade _photoFacade { get; set; } = null!;
    [Inject] private ToastService _toastService { get; set; } = null!;

    [Inject] private DateTimeProvider _dateTimeProvider { get; set; } = null!;

    private DailyMenuDTO? todayMenu;
    // private bool isLoading = false;

    //Model
    private Guid selectedFoodId = default;

    private MediaFile? capturedPhoto;
    private string capturedPhotoDisplayData = "";

    protected override async Task OnInitializedAsync()
    {
        // Get todays daily menu
        var today = _dateTimeProvider.Today;

        var availableMenus = await _menuAndOrderCacheService.CachedMenusAsync();

        var todayWeek = availableMenus.FirstOrDefault(m => m.DailyMenus.Any(d => d.Date == today));
        if (todayWeek == default)
        {
            //Todo:
            return;
        }

        todayMenu = todayWeek.DailyMenus.FirstOrDefault(d => d.Date == today);
        if (todayMenu is null)
        {
            //Todo:
            return;
        }

        // Preselect our ordered item
        var ordersForWeeks = await _menuAndOrderCacheService.CachedOrdersForWeeksAsync();
        var orders = ordersForWeeks[todayWeek.PrimirestMenuId];
        var todayOrder = orders.FirstOrDefault(o => todayMenu.Foods.Any(f => f.FoodId == o.SharpFoodId));

        if (todayOrder is not null)
        {
            selectedFoodId = todayOrder.SharpFoodId;
        }

        StateHasChanged();
    }

    /// <summary>
    /// Set the <see cref="capturedPhoto"/> to the photo taken by the user along with <see cref="capturedPhotoDisplayData"/> and call state has changed
    /// </summary>
    /// <returns></returns>
    private async Task CapturePhoto()
    {
        async Task<string> DisplayDataFrom(MediaFile photo)
        {
            var imageBytes = await File.ReadAllBytesAsync(photo.Path);
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }

#if DEBUG
        //Take a mock image from documents if on windows and in debug
        //So we don't have to deal with camera stuff on windows
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            //Mock image from documents
            var mockImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "mock.jpg");
            var stream = File.OpenRead(mockImagePath);

            capturedPhoto = new MediaFile(
                mockImagePath,
                () => stream,
                originalFilename: "mock.jpg");

            capturedPhotoDisplayData = await DisplayDataFrom(capturedPhoto);

            return;
        }
#endif
        var newPhoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
        {
            AllowCropping = true,
            DefaultCamera = CameraDevice.Front,
            PhotoSize = PhotoSize.MaxWidthHeight,
            MaxWidthHeight = 1024,
            Name = "photo.jpg",
        });

        if (newPhoto is null)
            return;

        //Set new photo
        capturedPhoto = newPhoto;
        capturedPhotoDisplayData = await DisplayDataFrom(capturedPhoto);

        StateHasChanged();
    }

    private async void TryPublishPhoto()
    {
        //Validation
        if (capturedPhoto is null)
            return;

        if (selectedFoodId == default)
            return;


        //Publish
        var result = await _photoFacade.PublishPhotoAsync(selectedFoodId, capturedPhoto);
        if (result is null)
        {
            //No error
            await _toastService.ShowSuccessAsync("Yipee!"); //Todo: replace with something more amazing
            return;
        }

        //Error, show it
        await _toastService.ShowErrorAsync(result.Value.Title);
    }
}