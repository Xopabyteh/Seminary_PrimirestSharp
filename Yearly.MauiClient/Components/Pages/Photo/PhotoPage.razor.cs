using Microsoft.AspNetCore.Components;
using Plugin.Media.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;
using Image = SixLabors.ImageSharp.Image;

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

    private MediaFile? capturedPhotoRaw;
    private MemoryStream? processedPhotoStream;
    private string? capturedPhotoDisplayData = "";

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
    /// Set the <see cref="processedPhotoStream"/> to the photo taken by the user along with <see cref="capturedPhotoDisplayData"/> and call state has changed
    /// </summary>
    /// <returns></returns>
    private async Task CapturePhoto()
    {
        //async Task<string> DisplayDataFrom(MediaFile photo)
        //{
        //    var imageBytes = await File.ReadAllBytesAsync(photo.Path);
        //    return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        //}
        async Task<string> DisplayDataFrom(Stream photoStream)
        {
            var imageBytes = new byte[photoStream.Length];
            _ = await photoStream.ReadAsync(imageBytes);
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }

#if DEBUG && WINDOWS
        //Take a mock image from documents if on windows and in debug
        //So we don't have to deal with camera stuff on windows

        //Mock image from documents
        var mockImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "mock.jpg");
        var stream = File.OpenRead(mockImagePath);

        var newPhoto = new MediaFile(
            mockImagePath,
            () => stream,
            originalFilename: "mock.jpg");

        // ReSharper disable once RedundantJumpStatement [rev: only applies to windows debug scope]
        goto skipPhotoTake; //A little dirty, but it's only dev code anyway
#else
    // Android/IOS or not debug
        
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

#endif
#if DEBUG && WINDOWS
    skipPhotoTake:
#endif

        //Set new photo
        capturedPhotoRaw = newPhoto;

        //Crop photo from sides so it's 1:1 aspect ratio

        //Load to stream
        var rawPhotoReadStream = newPhoto.GetStreamWithImageRotatedForExternalStorage();
        using var photoStreamForMutation = new MemoryStream();
        await rawPhotoReadStream.CopyToAsync(photoStreamForMutation);
        photoStreamForMutation.Position = 0; //Set for reading

        //Mutate
        var image = await Image.LoadAsync(photoStreamForMutation);
        var minDimension = Math.Min(image.Width, image.Height);
        var cropRectangle = new Rectangle(
            (image.Width - minDimension) / 2,
            (image.Height - minDimension) / 2,
            minDimension,
            minDimension);

        image.Mutate(x => x.Crop(cropRectangle));

        //Save
        processedPhotoStream = new();
        var imageEncoder = new JpegEncoder();
        await image.SaveAsync(processedPhotoStream , imageEncoder);

        //Display data
        processedPhotoStream.Position = 0; //Set for reading
        capturedPhotoDisplayData = await DisplayDataFrom(processedPhotoStream);
        StateHasChanged();
    }

    private async void TryPublishPhoto()
    {
        //Validation
        if (processedPhotoStream is null)
            return;

        if (selectedFoodId == default)
            return;


        //Publish
        processedPhotoStream.Position = 0; //Set for reading
        var result = await _photoFacade.PublishPhotoAsync(
            selectedFoodId,
            processedPhotoStream,
            capturedPhotoRaw!.OriginalFilename);

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