using System.Net;

namespace Yearly.Application.Services.Authentication;

public interface IPrimirestAuthService
{
    public Task<AuthenticationResult> LoginAsync(string username, string password);
    public Task LogoutAsync(string sessionCookie);
    public Task<PrimirestUser> GetPrimirestUserInfoAsync(string sessionCookie);
}