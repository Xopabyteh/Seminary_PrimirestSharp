using System.Net.Http.Json;
using Yearly.Contracts.Authentication;

namespace Yearly.MauiClient.Services;

public class SharpAPIFacade
{
    private readonly HttpClient _httpClient;
    private const string k_BaseAddress = "https://localhost:7217";

    private readonly AuthService _authService;
    public SharpAPIFacade(AuthService authService)
    {
        _authService = authService;

        _httpClient = new();
        _httpClient.BaseAddress = new Uri(k_BaseAddress);
    }

    /// <summary>
    /// Calls login, sets session cookie in <see cref="AuthService.SetSessionAsync"/> and returns the response.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            await _authService.SetSessionAsync(result);

            return result;
        }
        else
        {
            //:(
        }

        return default;
    }
}