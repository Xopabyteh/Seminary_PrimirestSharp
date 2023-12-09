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

    /// <summary>
    /// Sets the session cookie header for the client, so it can be used for authenticated requests.
    /// </summary>
    /// <param name="sessionCookie"></param>
    public void SetSessionCookie(string sessionCookie)
    {
        _httpClient.DefaultRequestHeaders.Remove("SessionCookie"); //Remove old cookie if any
        _httpClient.DefaultRequestHeaders.Add("SessionCookie", sessionCookie);
    }
}