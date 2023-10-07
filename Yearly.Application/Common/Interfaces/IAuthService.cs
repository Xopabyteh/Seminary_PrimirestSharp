using ErrorOr;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Application.Authentication.Queries.PrimirestUser;

namespace Yearly.Application.Common.Interfaces;

public interface IAuthService
{
    public Task<ErrorOr<LoginResult>> LoginAsync(string username, string password);
    public Task LogoutAsync(string sessionCookie);
    public Task<ErrorOr<PrimirestUser>> GetUserInfoAsync(string sessionCookie);
}