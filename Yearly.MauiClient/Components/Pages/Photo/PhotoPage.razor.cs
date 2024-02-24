using Microsoft.AspNetCore.Components;
#if ANDROID || IOS
using Plugin.Media;
#endif
using Plugin.Media.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Components.Common;
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

    [Parameter, SupplyParameterFromQuery(Name = "selectedFoodId")] public string? SelectedFoodIdQuery { get; set; } = null;

    private DailyMenuDTO? selectedMenu;
    private Guid selectedFoodId = default;

    private MediaFile? capturedPhotoRaw;
    private MemoryStream? processedPhotoStream;
    private string? capturedPhotoDisplayData = "";

    private bool showSuccessModal = false;
    private bool fadeModalAway = false;

    private GoBackButton goBackButton = null!; //@ref

    private bool canTakePhotoToday = true;
    private bool optionsLoaded = false;
    protected override async Task OnInitializedAsync()
    {
        // Preselect our item. If we're coming with a selected food already -> select it,
        // otherwise select the food we have ordered
        // if no order, don't select anything
        if (SelectedFoodIdQuery is not null && Guid.TryParse(SelectedFoodIdQuery, out selectedFoodId))
        {
            await SelectBasedOnQuery();
        }
        else
        {
            await SelectBasedOnToday();
        }
    }

    private async Task SelectBasedOnToday()
    {
        // Get todays daily menu
        var today = _dateTimeProvider.Today;

        var availableMenus = await _menuAndOrderCacheService.CachedMenusAsync();

        var todayWeeklyMenu = availableMenus.FirstOrDefault(m => m.DailyMenus.Any(d => d.Date == today));
        if (todayWeeklyMenu == default)
        {
            //No weekly menu today
            optionsLoaded = true;
            canTakePhotoToday = false;
            StateHasChanged();
            return;
        }

        selectedMenu = todayWeeklyMenu.DailyMenus.FirstOrDefault(d => d.Date == today);
        if (selectedMenu is null)
        {
            //No daily menu today
            optionsLoaded = true;
            canTakePhotoToday = false;
            StateHasChanged();
            return;
        }

        //Try to select food that we have ordered
        var ordersForWeeks = await _menuAndOrderCacheService.CachedOrdersForWeeksAsync();
        var orders = ordersForWeeks[todayWeeklyMenu.PrimirestMenuId];
        var todayOrder = orders.FirstOrDefault(o => selectedMenu.Foods.Any(f => f.FoodId == o.SharpFoodId));

        if (todayOrder is not null)
        {
            selectedFoodId = todayOrder.SharpFoodId;
        }

        canTakePhotoToday = true;
        optionsLoaded = true;
        StateHasChanged();
    }

    private async Task SelectBasedOnQuery()
    {
        //selectedFoodId is filled from query here
        var availableMenus = await _menuAndOrderCacheService.CachedMenusAsync();
        selectedMenu = availableMenus
            .SelectMany(m => m.DailyMenus)
            .Single(d => d.Foods.Any(f => f.FoodId == selectedFoodId));

        canTakePhotoToday = true;
        optionsLoaded = true;
        StateHasChanged();
    }

    private void SelectFoodId(Guid foodId) //Called by the card
    {
        selectedFoodId = foodId;
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

    private async Task TryPublishPhoto()
    {
        if (!await ValidateModel())
            return;

        //Initialize stream of data to send
        processedPhotoStream!.Position = 0; //Set for reading
        var sendStream = new MemoryStream();
        await processedPhotoStream.CopyToAsync(sendStream);

        // Show thank you modal assuming publish will result in success
        showSuccessModal = true;
        fadeModalAway = false;
        StateHasChanged();

        //Publish in background, hoping it goes right (it should [lol])
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            sendStream!.Position = 0; //Set for reading
            var result = await _photoFacade.PublishPhotoAsync(
                selectedFoodId,
                sendStream,
                capturedPhotoRaw!.OriginalFilename);

            if (result is not null)
            {
                //Error, show it
                await _toastService.ShowErrorAsync(result.Value.Title);
            }
        });
    }

    /// <summary>
    /// Tells the user what to do if the model is invalid via toasters
    /// </summary>
    /// <returns>Whether the model is valid</returns>
    private async Task<bool> ValidateModel()
    {
        //Validation
        if (processedPhotoStream is null)
        {
            //No photo
            await _toastService.ShowErrorAsync("Nejdøív udìlej fotku");
            return false;
        }

        if (selectedFoodId == default)
        {
            //No food selected
            await _toastService.ShowErrorAsync("Nejdøív vyber jídlo");
            return false;
        }

        return true;
    }

    private void OnAddAnotherPhotoClicked()
    {
        showSuccessModal = false;
        fadeModalAway = true;

        capturedPhotoRaw = null;
        processedPhotoStream = null;
        capturedPhotoDisplayData = null;
        
        selectedFoodId = default;
        StateHasChanged();
    }

    private Task GoBack()
    {
        return goBackButton.GoBack();
    }
}