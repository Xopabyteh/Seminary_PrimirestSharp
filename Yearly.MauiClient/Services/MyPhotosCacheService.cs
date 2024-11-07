using Yearly.Contracts.Common;
using Yearly.MauiClient.Services.SharpApi.Facades;

namespace Yearly.MauiClient.Services;

public class MyPhotosCacheService
{
    private readonly PhotoFacade _photoFacade;

    /// <summary>
    /// Key: PageOffset, Value: <see cref="DataFragmentDTO{PhotoLinkDTO}"/>
    /// </summary>
    private Dictionary<int, DataFragmentDTO<PhotoLinkDTO>> myPhotos = new(4);

    /// <summary>
    /// Lazily gets my photos using photoFacade and caches result
    /// </summary>
    /// <returns></returns>
    public async ValueTask<DataFragmentDTO<PhotoLinkDTO>> GetMyPhotosCachedAsync(int pageOffset)
    {
        if (myPhotos.TryGetValue(pageOffset, out var photosFragment))
            return photosFragment;

        //Cache miss
        var result = await _photoFacade.GetMyPhotosAsync(pageOffset);
        myPhotos.Add(pageOffset, result);

        return result;
    }

    public MyPhotosCacheService(PhotoFacade photoFacade)
    {
        _photoFacade = photoFacade;
    }

    public void InvalidateCache()
    {
        myPhotos.Clear();
    }
}