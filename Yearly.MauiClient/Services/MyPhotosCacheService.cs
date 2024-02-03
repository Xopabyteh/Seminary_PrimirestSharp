using Yearly.Contracts.Photos;
using Yearly.MauiClient.Services.SharpApi.Facades;

namespace Yearly.MauiClient.Services;

public class MyPhotosCacheService
{
    private readonly PhotoFacade _photoFacade;
    private MyPhotosResponse? myPhotos = null;

    /// <summary>
    /// Lazily gets my photos using photoFacade and caches result
    /// </summary>
    /// <returns></returns>
    public async ValueTask<MyPhotosResponse> GetMyPhotosCachedAsync()
    {
        if (myPhotos is not null)
            return myPhotos.Value;

        //Cache miss
        var result = await _photoFacade.GetMyPhotosAsync();
        myPhotos = result;

        return myPhotos.Value;
    }

    public MyPhotosCacheService(PhotoFacade photoFacade)
    {
        _photoFacade = photoFacade;
    }

    public void InvalidateCache()
    {
        myPhotos = null;
    }
}