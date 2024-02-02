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

        var response = await _sharpAPIClient.HttpClient.PostAsync("/api/photo/publish", content);

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
        var response = await _sharpAPIClient.HttpClient.GetAsync("/api/photo/waiting");
        response.EnsureSuccessStatusCode();

        var waitingPhotos = await response.Content.ReadFromJsonAsync<WaitingPhotosResponse>();
        return waitingPhotos?.Photos ?? new();
    }

    /// <summary>
    /// Can return "Photo.PhotoNotFound"
    /// </summary>
    /// <param name="photoId"></param>
    /// <returns>Null if success or problem response</returns>
    public async Task<ProblemResponse?> ApprovePhotoAsync(Guid photoId)
    {
        //Post to {{host}}/photo/approve
        //With form data: photoId = photoId

        var content = new MultipartFormDataContent
        {
            {new StringContent(photoId.ToString()), "photoId"}
        };

        var response = await _sharpAPIClient.HttpClient.PostAsync("/api/photo/approve", content);

        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        return problem;
    }

    /// <summary>
    /// Can return "Photo.PhotoNotFound"
    /// </summary>
    /// <returns>Null if success or problem response</returns>
    public async Task<ProblemResponse?> RejectPhotoAsync(Guid photoId)
    {
        //Post to {{host}}/photo/reject
        //With form data: photoId = photoId

        var content = new MultipartFormDataContent
        {
            {new StringContent(photoId.ToString()), "photoId"}
        };

        var response = await _sharpAPIClient.HttpClient.PostAsync("/api/photo/reject", content);

        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        return problem;
    }

    public async Task<MyPhotosResponse> GetMyPhotosAsync()
    {
        var response = await _sharpAPIClient.HttpClient.GetAsync("/api/photo/my-photos");
        response.EnsureSuccessStatusCode();

        var myPhotos = await response.Content.ReadFromJsonAsync<MyPhotosResponse>();
        return myPhotos;
    }
}