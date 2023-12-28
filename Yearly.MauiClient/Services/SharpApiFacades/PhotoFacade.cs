using Plugin.Media.Abstractions;
using System.Net.Http.Json;
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
}