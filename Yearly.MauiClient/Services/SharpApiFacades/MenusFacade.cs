using System.Net.Http.Json;
using Yearly.Contracts.Menu;

namespace Yearly.MauiClient.Services.SharpApiFacades;

public class MenusFacade
{
    private readonly SharpAPIClient _sharpAPIClient;

    public MenusFacade(SharpAPIClient sharpAPIClient)
    {
        _sharpAPIClient = sharpAPIClient;
    }

    public async Task<AvailableMenusResponse> GetAvailableMenusAsync()
    {
        var response = await _sharpAPIClient.HttpClient.GetAsync("/menu/available");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AvailableMenusResponse>();
            return result!;
        }
        else
        {
            //Todo: :(
        }

        return new(new());
    }
}