using System.Net.Http.Json;
using Yearly.Contracts.Authentication;

namespace Yearly.MauiClient.Services.SharpApiFacades;

public class AuthenticationFacade
{
    private readonly SharpAPIClient _sharpAPIClient;

    public AuthenticationFacade(SharpAPIClient sharpAPIClient)
    {
        _sharpAPIClient = sharpAPIClient;
    }

    /// <summary>
    /// Calls login on the API and returns the login response.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _sharpAPIClient.HttpClient.PostAsJsonAsync("/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return result;
        }
        else
        {
            //Todo: :(
        }

        return default;
    }

    public async Task<UserDetailsResponse?> GetMyDetailsAsync()
    {
        var response = await _sharpAPIClient.HttpClient.GetAsync("/auth/my-details");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UserDetailsResponse>();
            return result;
        }

        return null;
    }
}