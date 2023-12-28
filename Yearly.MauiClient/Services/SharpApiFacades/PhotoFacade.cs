using Plugin.Media.Abstractions;
using System.Net.Http.Json;
using Yearly.Contracts.Photos;
using Yearly.MauiClient.Exceptions;

namespace Yearly.MauiClient.Services.SharpApiFacades;

public class PhotoFacade
{
    private readonly SharpAPIClient _sharpAPIClient;

    public PhotoFacade(SharpAPIClient sharpAPIClient)
    {
        _sharpAPIClient = sharpAPIClient;
    }

    public async Task<ProblemResponse?> PublishPhotoAsync(Guid foodId, MediaFile photo)
    {
        //Convert photo to IFormFile
        var content = new MultipartFormDataContent
        {
            {new StreamContent(photo.GetStream()), "photo", photo.OriginalFilename},
            {new StringContent(foodId.ToString()), "foodId"}
        };

        var response = await _sharpAPIClient.HttpClient.PostAsync("/photo/publish", content);

        //Consider the request was validated and the response should be success
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
            return problem;
        }

        return null;
    }

    /// <summary>
    /// Get's photos that are waiting for approval
    /// </summary>
    /// <returns>A List of photos waiting for approval, may be empty</returns>
    public async Task<List<PhotoDTO>> GetWaitingPhotosAsync()
    {
        var response = await _sharpAPIClient.HttpClient.GetAsync("/photo/waiting");
        response.EnsureSuccessStatusCode();

        var waitingPhotos = await response.Content.ReadFromJsonAsync<WaitingPhotosResponse>();
        return waitingPhotos?.Photos ?? new();
    }
}