using System.Net.Http.Json;
using Yearly.Contracts.Foods;

namespace Yearly.MauiClient.Services.SharpApi.Facades;


public class FoodFacade
{
    private readonly SharpAPIClient _client;

    public FoodFacade(SharpAPIClient client)
    {
        _client = client;
    }

    public async Task<FoodSimilarityTableResponse> GetSimilarityTableAsync()
    {
        //Get {{host}}/food/similarity-table

        var response = await _client.HttpClient.GetAsync("/api/food/similarity-table");

        response.EnsureSuccessStatusCode();

        var similarityTable = await response.Content.ReadFromJsonAsync<FoodSimilarityTableResponse>();
        return similarityTable!;
    }

    /// <summary>
    /// Sets the ofFood as an alias for the forFood.
    /// forFood acts as the potential alias origin.
    /// </summary>
    /// <param name="ofFoodId">The newly persisted food</param>
    /// <param name="forFoodId">The potential alias for the food</param>
    /// <returns></returns>
    public async Task ApproveSimilarityAsync(Guid ofFoodId, Guid forFoodId)
    {
        //Post {{host}}/food/similarity-table/approve

        var content = new MultipartFormDataContent
        {
            {new StringContent(ofFoodId.ToString()), "ofFood"},
            {new StringContent(forFoodId.ToString()), "forFood"}
        };

        var response = await _client.HttpClient.PostAsync("/api/food/similarity-table/approve", content);

        response.EnsureSuccessStatusCode();
    }
    /// <summary>
    /// Disapproves the similarity between the two foods.
    /// The ofFood is the newly persisted food.
    /// The forFood is the potential alias origin.
    /// </summary>
    /// <param name="ofFoodId">The newly persisted food</param>
    /// <param name="forFoodId">The potential alias for the food</param>
    /// <returns></returns>
    public async Task DisapproveSimilarityAsync(Guid ofFoodId, Guid forFoodId)
    {
        //Post {{host}}/food/similarity-table/disapprove

        var content = new MultipartFormDataContent
        {
            {new StringContent(ofFoodId.ToString()), "ofFood"},
            {new StringContent(forFoodId.ToString()), "forFood"}
        };

        var response = await _client.HttpClient.PostAsync("/api/food/similarity-table/disapprove", content);

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Sets the alias for the food.
    /// </summary>
    /// <param name="ofFoodId">The food to set the alias for</param>
    /// <param name="forFoodId">The alias origin</param>
    public async Task SetAliasAsync(Guid ofFoodId, Guid forFoodId)
    {
        //Post {{host}}/food/set-alias

        var content = new MultipartFormDataContent
        {
            {new StringContent(ofFoodId.ToString()), "ofFood"},
            {new StringContent(forFoodId.ToString()), "forFood"}
        };

        var response = await _client.HttpClient.PostAsync("/api/food/set-alias", content);

        response.EnsureSuccessStatusCode();
    }
}