using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.Contracts.Common;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Photo;

public partial class MyPhotosPage
{
    [Inject] private MyPhotosCacheService MyPhotosCacheService { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private bool firstLoadFinished = false;
    private bool loadingMorePhotos = false;

    private ElementReference photosGridRef; //@ref
    private DotNetObjectReference<MyPhotosPage>? pageRefDotNetHelper;

    private List<PhotoLinkDTO> displayedPhotos = new(20);
    private int totalPhotoCount = 0;

    protected override void OnInitialized()
    {
        pageRefDotNetHelper = DotNetObjectReference.Create(this);
    }

    protected override async Task OnInitializedAsync()
    {
        var firstPhotosFragment = await MyPhotosCacheService.GetMyPhotosCachedAsync(pageOffset: 0);
        displayedPhotos.AddRange(firstPhotosFragment.Data);
        totalPhotoCount = firstPhotosFragment.TotalCount;

        firstLoadFinished = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await JS.InvokeVoidAsync(
            "MyPhotosPage.initializeVirtualizationScrollChecker", 
            photosGridRef, 
            pageRefDotNetHelper);
    }

    [JSInvokable]
    public async Task LoadMorePhotos()
    {
        if (loadingMorePhotos)
            return;

        if (displayedPhotos.Count == totalPhotoCount)
            return;

        loadingMorePhotos = true;
        StateHasChanged();

        var photosFragment = await MyPhotosCacheService.GetMyPhotosCachedAsync(pageOffset: displayedPhotos.Count);
        displayedPhotos.AddRange(photosFragment.Data);
        loadingMorePhotos = false;
        StateHasChanged();
    }

    private string PhotosTextOnlyGrammar()
    {
        return totalPhotoCount switch
        {
            1 => "Fotka",
            < 5 => "Fotky",
            _ => "Fotek"
        };
    }
}