using System.Net.Http.Json;
using Yearly.Contracts.Authentication;

namespace Yearly.MauiClient.Services.SharpApiFacades;

public class AuthenticationFacade
{
    private readonly SharpAPIClient _sharpAPIClient;
    private readonly AuthService _authService;

    public AuthenticationFacade(SharpAPIClient sharpAPIClient, AuthService authService)
    {
        _sharpAPIClient = sharpAPIClient;
        _authService = authService;
    }

    /// <summary>
    /// Calls login, sets session cookie in <see cref="AuthService.SetSessionAsync"/>,
    /// sets SessionCookie on the <see cref="SharpAPIClient.HttpClient"/>, making it usable for authenticated actions
    /// and returns the login response.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _sharpAPIClient.HttpClient.PostAsJsonAsync("/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            await _authService.SetSessionAsync(result);
            _sharpAPIClient.HttpClient.DefaultRequestHeaders.Add("SessionCookie", result.SessionCookie);

            return result;
        }
        else
        {
            //Todo: :(
        }

        return default;
    }
}