using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using Pipelines.Sockets.Unofficial.Arenas;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Infrastructure.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Yearly.Infrastructure.Services.Authentication;

public class PrimirestAuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    public PrimirestAuthService(IHttpClientFactory httpClientFactory, IDateTimeProvider dateTimeProvider)
    {
        _httpClientFactory = httpClientFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Creates a session cookie and attempts to mark it as logged in by sending a login request to Primirest.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<ErrorOr<string>> LoginAsync(string username, string password)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        var sessionCookie = CreateValidCookie();

        var requestContent = new FormUrlEncodedContent(new [] {
            new KeyValuePair<string?, string?>("UserName", username),
            new KeyValuePair<string?, string?>("Password", password),
            new KeyValuePair<string?, string?>("RememberMe", "false"),
        });

        Activity.Current = null; // Used to remove traceparent header
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "ajax/EN/auth/login")
        {
            Content = requestContent,
            Headers = { 
                { "Cookie", sessionCookie },
                { "Connection", "keep-alive" },
                { "Cache-Control", "max-age=0" },
                { "sec-ch-ua", "\"Not/A)Brand\";v=\"8\", \"Chromium\";v=\"126\", \"Brave\";v=\"126\"" },
                { "sec-ch-ua-mobile", "?0" },
                { "sec-ch-ua-platform", "\"Windows\"" },
                { "Upgrade-Insecure-Requests", "1" },
                { "Origin", "https://mujprimirest.cz" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36" },
                { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8" },
                { "Sec-GPC", "1" },
                { "Accept-Language", "en-US,en;q=0.8" },
                { "Sec-Fetch-Site", "same-origin" },
                { "Sec-Fetch-Mode", "navigate" },
                { "Sec-Fetch-User", "?1" },
                { "Sec-Fetch-Dest", "document" },
                { "Referer", "https://mujprimirest.cz/CS/auth/login" },
                { "Accept-Encoding", "gzip, deflate, br, zstd" }
            },
        };

        var loginResponse = await client.SendAsync(requestMessage);

        if (loginResponse.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.InvalidCredentials;

        return sessionCookie;
    }

    public async Task LogoutAsync(string sessionCookie)
    {
        const string logoutPath = "ajax/CS/auth/logout";
        
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, logoutPath);
        requestMessage.Headers.Add("cookie", sessionCookie);
        await client.SendAsync(requestMessage);
    }

    public async Task<ErrorOr<PrimirestUserInfo[]>> GetAvailableUsersInfoAsync(string sessionCookie)
    {
        var timeStamp = ((DateTimeOffset)_dateTimeProvider.UtcNow).ToUnixTimeSeconds();

        var path = $"cs/context/available?q=&_={timeStamp}";
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
        requestMessage.Headers.Add("cookie", sessionCookie);

        var response = await client.SendAsync(requestMessage);

        var resultJson = await response.Content.ReadAsStringAsync();

        if (response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        var usersWithinTenantResponse = JsonConvert.DeserializeObject<AvailableResponseRoot>(resultJson) ?? throw new InvalidOperationException();

        return usersWithinTenantResponse.Items
            .Select(user => new PrimirestUserInfo(
                user.ID,
                user.Name))
            .ToArray();
    }

    public async Task<ErrorOr<Unit>> SwitchPrimirestContextAsync(
        string sessionCookie,
        UserId newUserId)
    {
        var path = $"en/context/switch/{newUserId.Value}";
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
        requestMessage.Headers.Add("cookie", sessionCookie);

        var response = await client.SendAsync(requestMessage);

        if(response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        return Unit.Value;
    }

    /// <summary>
    /// Creates a cookie with exactly 24 bytes of value
    /// that can be used as a session token for Primirest.
    /// </summary>
    private string CreateValidCookie()
    {
        // Todo: do better :D
        ReadOnlySpan<byte> cookieBytes = RandomNumberGenerator.GetBytes(16);
        ReadOnlySpan<char> cookieValue = Convert.ToBase64String(cookieBytes).ToLower();
        Span<char> urlFriendlyCookieValue = stackalloc char[24];
        for (int i = 0; i < 22; i++)
        {
            //Todo: change this weird thing to something more reliable
            urlFriendlyCookieValue[i] = cookieValue[i] switch
            {
                '+' => 'x',
                '/' => 'x',
                '=' => 'x',
                '1' => 'a',
                '2' => 'b',
                '3' => 'c',
                '4' => 'd',
                '5' => 'e',
                '6' => 'f',
                '7' => 'g',
                '8' => 'h',
                '9' => 'i',
                '0' => 'j',
                _ => cookieValue[i]
            };
        }

        urlFriendlyCookieValue[22] = 'x';
        urlFriendlyCookieValue[23] = 'x';

        //var cookie = new Cookie("ASP.NET_SessionId", new string(urlFriendlyCookieValue));
        return $"ASP.NET_SessionId={new string(urlFriendlyCookieValue)}";
    }


    // Used for easier json serialization
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Used for json serialization")]
    private record PrimirestLoginRequest(
        string Username,
        string Password);
}