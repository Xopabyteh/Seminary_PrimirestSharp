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

    public Task<ErrorOr<PrimirestUserInfo>> GetPrimirestUserInfoAsync(string sessionCookie)
    {
        //Little dirty, i know
        return Task.FromResult((ErrorOr<PrimirestUserInfo>)new PrimirestUserInfo(26564871, "Martin Fiala"));
    }

    public Task<ErrorOr<Unit>> SwitchPrimirestContextAsync(UserId newUserId)
    {
        // NOOP
        return Task.FromResult((ErrorOr<Unit>)Unit.Value);
    }
}