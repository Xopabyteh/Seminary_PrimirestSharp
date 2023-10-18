using ErrorOr;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication;

public interface IAuthService
{
    public Task<ErrorOr<LoginResult>> LoginAsync(string username, string password);
    public Task LogoutAsync(string sessionCookie);
    public Task<ErrorOr<User>> GetUserInfoAsync(string sessionCookie);
    public Task<UserRoles> GetUserRoleAsync(UserId userId);
}