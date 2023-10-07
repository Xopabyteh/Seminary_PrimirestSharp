using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using Newtonsoft.Json;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Application.Authentication.Queries.PrimirestUser;
using Yearly.Application.Common.Interfaces;
using Yearly.Application.Errors;
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
    /// There response from Primirest is always OK, so we can't check if the login was successful.
    /// <b>You have to check that the cookie was marked yourself.</b>
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<ErrorOr<LoginResult>> LoginAsync(string username, string password)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        var sessionCookie = CreateValidCookie();

        var requestContent = new StringContent(
            JsonConvert.SerializeObject(new PrimirestLoginRequest(username, password)),
            Encoding.UTF8,
            "application/json"
        );

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "ajax/CS/auth/login")
        {
            Content = requestContent,
        };
        requestMessage.Headers.Add("cookie", sessionCookie);

        //Todo: maybe there is a way to check it
        //Response is always OK, se we can't check it...
        await client.SendAsync(requestMessage);  
        return new LoginResult(sessionCookie);
    }

    public Task LogoutAsync(string sessionCookie)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<PrimirestUser>> GetUserInfoAsync(string sessionCookie)
    {
        var timeStamp = ((DateTimeOffset)_dateTimeProvider.UtcNow).ToUnixTimeSeconds();

        var path = $"cs/context/available?q=&_={timeStamp}";
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
        requestMessage.Headers.Add("cookie", sessionCookie);

        var response = await client.SendAsync(requestMessage);

        var resultJson = await response.Content.ReadAsStringAsync();

        if (resultJson.StartsWith("<!doctype html>"))
        {
            //We have been redirected to the login page, so the cookie is not valid
            return Errors.Authentication.CookieNotSigned;
        }

        dynamic userObj = JsonConvert.DeserializeObject(resultJson) ?? throw new InvalidOperationException();
        dynamic userDetailsObj = userObj.Items[0];
        
        return new PrimirestUser(userDetailsObj.ID.ToString(), userDetailsObj.Name.ToString());
    }

    /// <summary>
    /// Creates a cookie with exactly 24 bytes of value
    /// that can be used as a session token for Primirest.
    /// </summary>
    private string CreateValidCookie()
    {
        ReadOnlySpan<byte> cookieBytes = RandomNumberGenerator.GetBytes(16);
        ReadOnlySpan<char> cookieValue = Convert.ToBase64String(cookieBytes).ToLower();
        Span<char> urlFriendlyCookieValue = stackalloc char[24];
        for (int i = 0; i < 22; i++)
        {
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
    private record PrimirestLoginRequest(
        string Username,
        string Password);
}