using ErrorOr;
using MediatR;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;

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

    public Task<ErrorOr<PrimirestUserInfo[]>> GetAvailableUsersInfoAsync(string sessionCookie)
    {
        // Little dirty, I know
        var availableUsers = new[] {new PrimirestUserInfo(26564871, "Martin Fiala")};
        return Task.FromResult((ErrorOr<PrimirestUserInfo[]>)availableUsers);
    }


    public Task<ErrorOr<Unit>> SwitchPrimirestContextAsync(string sessionCookie, UserId newUserId)
    {
        // NOOP
        return Task.FromResult((ErrorOr<Unit>)Unit.Value);
    }
}