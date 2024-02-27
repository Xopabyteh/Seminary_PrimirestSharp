using System.Net;
using Yearly.Contracts.Authentication;

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
        var handler = new SafeConnectionAwareHttpClientHandler() {UseCookies = true};
        var httpClient = new HttpClient(handler);

#if ANDROID
        //const string baseAddressLocalHost = "http://10.0.2.2:5281";
        //const string baseAddressLocalHost = "https://localhost:7217";
        const string baseAddressLocalHost = "https://ntg29zg8-7217.euw.devtunnels.ms";
        httpClient.BaseAddress = new Uri(baseAddressLocalHost);
#endif
#if WINDOWS
        //const string baseAddressLocalHost = "https://localhost:7217";
        const string baseAddressLocalHost = "https://ntg29zg8-7217.euw.devtunnels.ms";
        httpClient.BaseAddress = new Uri(baseAddressLocalHost);
#endif
#if IOS

#endif

#endif

#if RELEASE
        //Release:
        var handler = new SafeConnectionAwareHttpClientHandler() {UseCookies = true};
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

    public Cookie SetSessionCookie(SessionCookieDetails details)
    {
        var cookieContainer = HttpClientHandler.CookieContainer;
        var cookie = new Cookie(SessionCookieDetails.Name, details.Value) {Expires = details.ExpirationDate.Date};
        cookieContainer.Add(HttpClient.BaseAddress!, cookie);

        return cookie;
    }

    public void RemoveCookie(Cookie cookie)
    {
        HttpClientHandler.CookieContainer.GetCookies(HttpClient.BaseAddress!).Remove(cookie);
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