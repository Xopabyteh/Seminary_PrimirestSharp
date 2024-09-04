namespace Yearly.MauiClient.Services.SharpApi;

public class SharpAPIClient : IDisposable
{
    private readonly ApiUrlService _apiUrlService;

    public SafeConnectionAwareHttpClientHandler HttpClientHandler { get; init; }
    public HttpClient HttpClient { get; init; }


    public SharpAPIClient(ApiUrlService apiUrlService)
    {
        _apiUrlService = apiUrlService;
        (HttpClient, HttpClientHandler) = CreateClient();
    }

    private (HttpClient client, SafeConnectionAwareHttpClientHandler handler) CreateClient()
    {
        var handler = new SafeConnectionAwareHttpClientHandler() { UseCookies = true };
        var httpClient = new HttpClient(handler);

        httpClient.BaseAddress = new Uri(_apiUrlService.GetBaseAddress());

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
/// when not connected to internet.
/// </summary>
public class SafeConnectionAwareHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
#if ANDROID || IOS //Shit doesn't work on windows
        var canAccessInternet = Connectivity.NetworkAccess == NetworkAccess.Internet;
        if (!canAccessInternet)
        {
            throw new NoInternetAccessException();
        }
#endif
      
        var response = await base.SendAsync(request, cancellationToken);
        
        return response;
    }

    public class NoInternetAccessException : Exception { }
}