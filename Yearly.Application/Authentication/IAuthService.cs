using ErrorOr;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication;

public interface IAuthService
{
    /// <summary>
    /// Logs in user through external provider and returns SessionCookie which can be used to identify the user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns>SessionCookie</returns>
    public Task<ErrorOr<string>> LoginAsync(string username, string password);
    public Task LogoutAsync(string sessionCookie);

    /// <summary>
    /// Gets info available from external auth provider about the logged in user
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <returns></returns>
    public Task<ErrorOr<ExternalUserInfo>> GetExternalUserInfoAsync(string sessionCookie);

    /// <summary>
    /// Gets our applications info about the logged user from our repository.
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <returns></returns>
    public Task<ErrorOr<User>> GetSharpUser(string sessionCookie);
}