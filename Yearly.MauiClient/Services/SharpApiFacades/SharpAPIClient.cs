namespace Yearly.MauiClient.Services.SharpApiFacades;

public class SharpAPIClient
{
    private readonly HttpClient _httpClient;
    public HttpClient HttpClient => _httpClient;

    private const string k_BaseAddress = "https://primirest-sharp-webapp.azurewebsites.net/api";

    public SharpAPIClient()
    {
#if DEBUG
    //Debug:
    #if ANDROID
            const string androidProxyIp = "10.0.2.2";
            var androidDevHandler = new AndroidDevHttpClientHandler(androidProxyIp, new string[]{ "localhost", "127.0.0.1" });
            _httpClient = new(androidDevHandler);

            const string baseAddressAndroidProxy = "http://10.0.2.2:5281/api";
            _httpClient.BaseAddress = new Uri(baseAddressAndroidProxy);
    #endif
    #if WINDOWS
            _httpClient = new();

            const string baseAddressLocalHost = "https://localhost:7217/api";
            _httpClient.BaseAddress = new Uri(baseAddressLocalHost);
    #endif
    #if IOS

    #endif
#endif

#if RELEASE
        //Release:
        _httpClient = new();
        _httpClient.BaseAddress = new Uri(k_BaseAddress);
#endif
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

#if ANDROID && DEBUG
/// <summary>
/// Changes ips from json string responses to android proxy ip,
/// so resources in responses which are hosted in a local environment
/// can be proxied to via the emulator using the given proxy.
///
/// Basically just replaces localhost (the given ips) to 10.0.2.2 (the given proxy)
/// </summary>
file class AndroidDevHttpClientHandler : HttpClientHandler
{
    private readonly string _androidProxyIp;
    private readonly string[] _hostIpsToReplace;
    public AndroidDevHttpClientHandler(string androidProxyIp, string[] hostIpsToReplace)
    {
        _androidProxyIp = androidProxyIp;
        _hostIpsToReplace = hostIpsToReplace;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.Content.Headers.ContentType is null ||
            response.Content.Headers.ContentType.MediaType != "application/json")
            return response;

        var contentStr = await response.Content.ReadAsStringAsync(cancellationToken);

        if (contentStr.Length == 0)
            return response;

        foreach (var ipToReplace in _hostIpsToReplace)
        {
            contentStr = contentStr
                .Replace(ipToReplace, _androidProxyIp);
        }

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(contentStr);
        var contentStream = new MemoryStream(contentBytes);
        response.Content = new StreamContent(contentStream);
        return response;
    }
}
#endif