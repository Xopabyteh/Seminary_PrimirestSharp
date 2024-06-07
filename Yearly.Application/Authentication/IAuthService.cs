using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication;

public interface IAuthService
{
    /// <summary>
    /// Logs in user through the primirest auth provider and returns SessionCookie which can be used to identify the user in their system
    /// </summary>
    /// <returns>SessionCookie</returns>
    public Task<ErrorOr<string>> LoginAsync(string username, string password);
    public Task LogoutAsync(string sessionCookie);

    /// <summary>
    /// Gets info available from the primirest auth provider about the logged in user
    /// Returns all available users within the "user tenant"
    /// </summary>
    public Task<ErrorOr<PrimirestUserInfo[]>> GetPrimirestUserInfoAsync(string sessionCookie);

    /// <summary>
    /// Primirest keeps a session of a user within a "user tenant". That sucks...
    /// We can get the available users within the tenant and switch between them.
    /// This ensures that operations performed will work with the user we really want.
    /// </summary>
    public Task<ErrorOr<Unit>> SwitchPrimirestContextAsync(string sessionCookie, UserId newUserId);
}