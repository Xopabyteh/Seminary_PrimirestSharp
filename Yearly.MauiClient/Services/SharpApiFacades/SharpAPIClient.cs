namespace Yearly.MauiClient.Services.SharpApiFacades;


public class SharpAPIClient
{
    private readonly HttpClient _httpClient;
    public HttpClient HttpClient => _httpClient;

    private const string k_BaseAddress = "https://localhost:7217";

    public SharpAPIClient()
    {
        _httpClient = new();
        _httpClient.BaseAddress = new Uri(k_BaseAddress);
    }
}