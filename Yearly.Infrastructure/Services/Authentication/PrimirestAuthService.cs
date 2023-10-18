using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Yearly.Application.Authentication;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Infrastructure.Http;

namespace Yearly.Infrastructure.Services.Authentication;

public class PrimirestAuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly PrimirestAdminCredentialsOptions _adminCredentials;
    public PrimirestAuthService(IHttpClientFactory httpClientFactory, IDateTimeProvider dateTimeProvider, IOptions<PrimirestAdminCredentialsOptions> adminCredentials)
    {
        _httpClientFactory = httpClientFactory;
        _dateTimeProvider = dateTimeProvider;
        _adminCredentials = adminCredentials.Value;
    }

    /// <summary>
    /// Creates a session cookie and attempts to mark it as logged in by sending a login request to Primirest.
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

        var loginResponse = await client.SendAsync(requestMessage);

        //If the loginResponse request uri absolute path is "/CS", then the login was successful
        //If it is "/CS/auth/login", then the login was unsuccessful
        if (loginResponse.RequestMessage?.RequestUri?.AbsolutePath == "/CS/auth/login")
            return Application.Errors.Errors.Authentication.InvalidCredentials;

        return new LoginResult(sessionCookie);
    }

    public async Task LogoutAsync(string sessionCookie)
    {
        const string logoutPath = "ajax/CS/auth/logout";
        
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, logoutPath);
        requestMessage.Headers.Add("cookie", sessionCookie);
        await client.SendAsync(requestMessage);
    }

    public async Task<ErrorOr<User>> GetUserInfoAsync(string sessionCookie)
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
            return Application.Errors.Errors.Authentication.CookieNotSigned;
        }

        dynamic userObj = JsonConvert.DeserializeObject(resultJson) ?? throw new InvalidOperationException();
        dynamic userDetailsObj = userObj.Items[0];
        
        //return new PrimirestUser(userDetailsObj.ID.ToString(), userDetailsObj.Name.ToString());
        var userId = new UserId(int.Parse(userDetailsObj.ID.ToString()));
        var userName = (string)userDetailsObj.Name.ToString();
        return new User(userId, userName);
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
    private record PrimirestLoginRequest(
        string Username,
        string Password);

    /// <summary>
    /// Login to primirest with the infrastructue admin credentials (Martin's credentials)
    /// Performs the given action
    /// Logs out the admin credentials
    /// </summary>
    /// <typeparam name="TResult">The type to be returned from the function</typeparam>
    /// <param name="action">The function to be called. It should be async. You receive a <see cref="HttpClient"/>
    /// created by the factory with the name <see cref="HttpClientNames.Primirest"/> and with the session cookie set.
    /// </param>
    /// <returns>Error or TResult</returns>
    internal async Task<ErrorOr<TResult>> PerformAdminLoggedSessionAsync<TResult>(Func<HttpClient, Task<ErrorOr<TResult>>> action)
    {
        //Login as admin
        var username = _adminCredentials.AdminUsername;
        var password  = _adminCredentials.AdminPassword;

        var loginResult = await LoginAsync(username, password);
        if (loginResult.IsError)
            return Infrastructure.Errors.Errors.PrimirestAdapter.InvalidAdminCredentials;


        var sessionCookie = loginResult.Value.SessionCookie;
        
        var primirestLoggedClient = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        primirestLoggedClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        //Perform the action
        var result = await action(primirestLoggedClient);

        //Logout
        await LogoutAsync(sessionCookie);
        primirestLoggedClient.DefaultRequestHeaders.Remove("Cookie");

        return result;
    }
}