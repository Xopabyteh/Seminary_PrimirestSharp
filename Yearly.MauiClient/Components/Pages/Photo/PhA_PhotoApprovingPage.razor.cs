using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Photos;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient.Components.Pages.Photo;

public partial class PhA_PhotoApprovingPage
{
    [Inject] private PhotoFacade PhotoFacade { get; set; } = null!;
    [Inject] private ToastService ToastService { get; set; } = null!;
    private List<PhotoDTO> waitingPhotos = new();
    private Dictionary<PhotoDTO, PhotoResolveState> photoResolveStates = new();

    private bool photosLoaded = false;

    protected override async Task OnInitializedAsync()
    {
        waitingPhotos = await PhotoFacade.GetWaitingPhotosAsync();
        foreach (var photo in waitingPhotos)
        {
            photoResolveStates.Add(photo, PhotoResolveState.Unresolved);
        }

        photosLoaded = true;
    }

    private async Task ApprovePhoto(PhotoDTO photo)
    {
        var response = await PhotoFacade.ApprovePhotoAsync(photo.Id);
        if (response is not null)
        {
            //Error
            await ToastService.ShowErrorAsync(response.Value.Title);
            return;
        }

        //Success
        photoResolveStates[photo] = PhotoResolveState.Approved;
        StateHasChanged();
    }

    private async Task RejectPhoto(PhotoDTO photo)
    {
        var response = await PhotoFacade.RejectPhotoAsync(photo.Id);
        if (response is not null)
        {
            //Error
            await ToastService.ShowErrorAsync(response.Value.Title);
            return;
        }

        //Success
        photoResolveStates[photo] = PhotoResolveState.Rejected;
        StateHasChanged();
    }

    private enum PhotoResolveState
    {
        Unresolved,
        Rejected,
        Approved
    }
}