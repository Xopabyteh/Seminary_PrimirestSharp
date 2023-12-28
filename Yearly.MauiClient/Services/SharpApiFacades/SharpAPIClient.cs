namespace Yearly.MauiClient.Services.SharpApiFacades;

public class SharpAPIClient
{
    private readonly HttpClient _httpClient;
    public HttpClient HttpClient => _httpClient;

    private const string k_BaseAddressUWP = "https://localhost:7217";
    private const string k_BaseAddressAndroid = "http://10.0.2.2:5281";
    private const string k_BaseAddressExternalDevice = "http://192.168.1.113:1337";

    public SharpAPIClient()
    {
        _httpClient = new();
        _httpClient.BaseAddress = new Uri(
            DeviceInfo.Platform == DevicePlatform.Android ? k_BaseAddressAndroid : k_BaseAddressUWP); //Todo: set link to cloud hosted
        //HttpClient.BaseAddress = new Uri(k_BaseAddressExternalDevice);
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