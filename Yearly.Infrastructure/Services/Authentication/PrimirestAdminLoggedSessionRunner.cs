using ErrorOr;
using Microsoft.Extensions.Options;
using Yearly.Application.Authentication;
using Yearly.Infrastructure.Http;

namespace Yearly.Infrastructure.Services.Authentication;

public class PrimirestAdminLoggedSessionRunner : IPrimirestAdminLoggedSessionRunner
{
    private readonly IAuthService _authService;
    private readonly PrimirestAdminCredentialsOptions _adminCredentials;
    private readonly IHttpClientFactory _httpClientFactory;

    public PrimirestAdminLoggedSessionRunner(
        IAuthService authService,
        IOptions<PrimirestAdminCredentialsOptions> adminCredentials,
        IHttpClientFactory httpClientFactory)
    {
        _authService = authService;
        _adminCredentials = adminCredentials.Value;
        _httpClientFactory = httpClientFactory;
    }

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
    public async Task<ErrorOr<TResult>> PerformAdminLoggedSessionAsync<TResult>(Func<HttpClient, Task<ErrorOr<TResult>>> action)
    {
        //Login as admin
        var username = _adminCredentials.AdminUsername;
        var password = _adminCredentials.AdminPassword;

        var loginResult = await _authService.LoginAsync(username, password);
        if (loginResult.IsError)
            return Errors.Errors.PrimirestAdapter.InvalidAdminCredentials;
        var sessionCookie = loginResult.Value;

        var primirestLoggedClient = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        primirestLoggedClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        //Perform the action
        var result = await action(primirestLoggedClient);

        //Logout
        await _authService.LogoutAsync(sessionCookie);
        primirestLoggedClient.DefaultRequestHeaders.Remove("Cookie");

        return result;
    }
}