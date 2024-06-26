using Microsoft.AspNetCore.Components;
#if ANDROID || IOS
using Plugin.Media;
#endif
using Plugin.Media.Abstractions;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Components.Common;
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

    [Parameter, SupplyParameterFromQuery(Name = "selectedFoodId")] public string? SelectedFoodIdQuery { get; set; } = null;

    private DailyMenuDTO? selectedMenu;
    private Guid selectedFoodId = default;

    private MediaFile? capturedPhotoRaw;
    private bool didCapturePhoto => capturedPhotoRaw is not null;
    private string? capturedPhotoDisplayData = "";

    private bool showSuccessModal = false;
    private bool fadeModalAway = false;

    private GoBackButton goBackButton = null!; //@ref

    private bool canTakePhotoToday = true;
    private bool optionsLoaded = false;

    private bool takingPhotoLoading = false;
    private bool isPublishingPhoto = false;
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

        await _menuAndOrderCacheService.EnsureMenusLoadedAsync();
        var availableMenus = _menuAndOrderCacheService.GetAvailableMenus();

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
        await _menuAndOrderCacheService.EnsureOrdersLoadedAsync(todayWeeklyMenu.PrimirestMenuId);
        var orders = _menuAndOrderCacheService.GetOrdersForWeek(todayWeeklyMenu.PrimirestMenuId);

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
        await _menuAndOrderCacheService.EnsureMenusLoadedAsync();
        var availableMenus = _menuAndOrderCacheService.GetAvailableMenus();
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

    private async Task CapturePhoto()
    {
        async Task<string> DisplayDataFrom(Stream photoStream)
        {
            var imageBytes = new byte[photoStream.Length];
            _ = await photoStream.ReadAsync(imageBytes);
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }

        takingPhotoLoading = true;
        StateHasChanged();
#if WINDOWS
        //Take a mock image from documents if on windows
        //So we don't have to deal with camera stuff on windows

        //Mock image from documents
        var mockImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "mock.jpg");

        var newPhoto = new MediaFile(
            mockImagePath,
            () => File.OpenRead(mockImagePath),
            originalFilename: "mock.jpg");

        // ReSharper disable once RedundantJumpStatement [rev: only applies to windows debug scope]
        goto skipPhotoTake; //A little dirty, but it's only dev code anyway
#else
        // Android/IOS or not debug
            
        var newPhoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
        {
            AllowCropping = true,
            DefaultCamera = CameraDevice.Front,
            PhotoSize = PhotoSize.Full,
            Name = "photo.jpg",
        });

        if (newPhoto is null)
        {
            takingPhotoLoading = false;
            StateHasChanged();
            return;
        }

#endif
#if WINDOWS
    skipPhotoTake:
#endif

        //Set new photo
        capturedPhotoRaw = newPhoto;

        //Display data
        capturedPhotoDisplayData = await DisplayDataFrom(capturedPhotoRaw.GetStream());

        takingPhotoLoading = false;
        StateHasChanged();
    }

    private async Task TryPublishPhoto()
    {
        if (isPublishingPhoto)
            return;

        isPublishingPhoto = true;
        StateHasChanged();

        if (!await ValidateModel())
        {
            isPublishingPhoto = true;
            return;
        }


        var result = await _photoFacade.PublishPhotoAsync(
            selectedFoodId,
            capturedPhotoRaw!.GetStream(),
            capturedPhotoRaw.OriginalFilename);

        if (result is not null)
        {
            //Error, show it
            isPublishingPhoto = false;
            await _toastService.ShowErrorAsync(result.Value.GetLocalizedMessage());
            return;
        }

        // -> Success
        showSuccessModal = true;
        fadeModalAway = false;
        StateHasChanged();
    }

    /// <summary>
    /// Tells the user what to do if the model is invalid via toasters
    /// </summary>
    /// <returns>Whether the model is valid</returns>
    private async Task<bool> ValidateModel()
    {
        //Validation
        if (capturedPhotoRaw is null)
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
        capturedPhotoDisplayData = null;
        
        selectedFoodId = default;
        isPublishingPhoto = false;
        StateHasChanged();
    }

    private void GoBack()
    {
        goBackButton.GoBack();
    }
}