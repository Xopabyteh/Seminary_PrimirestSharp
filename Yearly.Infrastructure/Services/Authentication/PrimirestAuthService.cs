using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Infrastructure.Http;

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

        var requestContent = new StringContent(
            JsonConvert.SerializeObject(new PrimirestLoginRequest(username, password)),
            Encoding.UTF8,
            "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "ajax/CS/auth/login")
        {
            Content = requestContent,
        };
        requestMessage.Headers.Add("cookie", sessionCookie);

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

    public async Task<ErrorOr<PrimirestUserInfo[]>> GetPrimirestUserInfoAsync(string sessionCookie)
    {
        var timeStamp = ((DateTimeOffset)_dateTimeProvider.UtcNow).ToUnixTimeSeconds();

        var path = $"cs/context/available?q=&_={timeStamp}";
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
        requestMessage.Headers.Add("cookie", sessionCookie);

        var response = await client.SendAsync(requestMessage);

        var resultJson = await response.Content.ReadAsStringAsync();

        if(response.GotRoutedToLogin())
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