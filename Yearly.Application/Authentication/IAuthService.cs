using ErrorOr;

namespace Yearly.Application.Authentication;

public interface IAuthService
{
    /// <summary>
    /// Logs in user through the primirest auth provider and returns SessionCookie which can be used to identify the user in their system
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns>SessionCookie</returns>
    public Task<ErrorOr<string>> LoginAsync(string username, string password);
    public Task LogoutAsync(string sessionCookie);

    /// <summary>
    /// Gets info available from the primirest auth provider about the logged in user
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <returns></returns>
    public Task<ErrorOr<PrimirestUserInfo>> GetPrimirestUserInfoAsync(string sessionCookie);

    ///// <summary>
    ///// Gets our applications info about the logged user from our repository.
    ///// </summary>
    ///// <param name="sessionCookie"></param>
    ///// <returns></returns>
    //public Task<ErrorOr<User>> GetSharpUserAsync(string sessionCookie);
}