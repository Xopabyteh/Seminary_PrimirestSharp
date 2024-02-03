namespace Yearly.MauiClient.Services.SharpApi;

public class SharpAPIClient : IDisposable
{

    public SafeConnectionAwareHttpClientHandler HttpClientHandler { get; init; }
    public HttpClient HttpClient { get; init; }

    public SharpAPIClient()
    {

        (HttpClient, HttpClientHandler) = CreateClient();
    }

    private (HttpClient client, SafeConnectionAwareHttpClientHandler handler) CreateClient()
    {
#if DEBUG
        //Debug:
        var handler = new SafeConnectionAwareHttpClientHandler();
        var httpClient = new HttpClient(handler);

#if ANDROID
        const string baseAddressLocalHost = "http://10.0.2.2:5281";
        httpClient.BaseAddress = new Uri(baseAddressLocalHost);
#endif
#if WINDOWS
        const string baseAddressLocalHost = "https://localhost:7217";
        httpClient.BaseAddress = new Uri(baseAddressLocalHost);
#endif
#if IOS

#endif

#endif

#if RELEASE
        //Release:
        var handler = new SafeConnectionAwareHttpClientHandler();
        var httpClient = new HttpClient(handler);

        const string baseAddressCloud = "https://primirest-sharp-webapp.azurewebsites.net";
        httpClient.BaseAddress = new Uri(baseAddressCloud);
#endif
        return (httpClient, handler);
    }

    public void Dispose()
    {
        HttpClientHandler.Dispose();
        HttpClient.Dispose();
    }
}

/// <summary>
/// Throws an exception when attempting to make a request
/// when not connected to internet
/// </summary>
public class SafeConnectionAwareHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var deviceNetAccess = Connectivity.Current.NetworkAccess;
        
        if (deviceNetAccess != NetworkAccess.Internet)
        {
            throw new NoInternetAccessException();
        }

        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }

    public class NoInternetAccessException : Exception;
}