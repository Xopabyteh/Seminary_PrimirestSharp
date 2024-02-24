using ErrorOr;
using Yearly.Application.Authentication;

namespace Yearly.Infrastructure.Services.Authentication;

public class AuthServiceDev : IAuthService
{
    public Task<ErrorOr<string>> LoginAsync(string username, string password)
    {
        return Task.FromResult((ErrorOr<string>)"debug");
    }

    public Task LogoutAsync(string sessionCookie)
    {
        // NOOP
        return Task.CompletedTask;
    }

    public Task<ErrorOr<PrimirestUserInfo>> GetPrimirestUserInfoAsync(string sessionCookie)
    {
        //Little dirty, i know
        return Task.FromResult((ErrorOr<PrimirestUserInfo>)new PrimirestUserInfo(26564871, "Martin Fiala"));
    }
}