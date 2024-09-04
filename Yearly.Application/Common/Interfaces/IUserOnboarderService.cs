using ErrorOr;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Common.Interfaces;

public interface IUserOnboarderService
{
    /// <summary>
    /// Creates or updates users in our system based on the users from primirest
    /// </summary>
    /// <param name="externalUserInfos">The users from primirest</param>
    /// <returns>
    /// A list of available onboarded/updated sharp-users within the login tenant
    /// </returns>
    public Task<ErrorOr<IReadOnlyCollection<User>>> OnboardOrUpdatePrimirestUsersToSharp(
        IReadOnlyCollection<PrimirestUserInfo> externalUserInfos);
}