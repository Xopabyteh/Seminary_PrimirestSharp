using ErrorOr;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Services.Authentication;

public class PrimirestToSharpUserOnboarderService : IUserOnboarderService
{
    private readonly IUserRepository _userRepository;

    public PrimirestToSharpUserOnboarderService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Does not save changes to db
    /// </summary>
    public async Task<ErrorOr<IReadOnlyCollection<User>>> OnboardOrUpdatePrimirestUsersToSharp(IReadOnlyCollection<PrimirestUserInfo> externalUserInfos)
    {
        if (externalUserInfos.Count == 0)
            return Error.Validation(
                "PrimirestToSharpUserOnboarderService.EmptyUserInfosCount",
                "The external user infos should contain >0 items");

        // Users from sharp (might not be in our system yet)
        var availableSharpUsers = await _userRepository.GetUsersByIdsAsync(
            externalUserInfos
                .Select(d => new UserId(d.Id))
                .ToArray()
            );

        // Onboard nonexistent users
        foreach (var externalUserDetails in externalUserInfos)
        {
            var userPricingGroup = externalUserDetails.DeterminePricingGroup();

            if (!availableSharpUsers.TryGetValue(new UserId(externalUserDetails.Id), out var sharpUser))
            {
                // Onboard
                sharpUser = new User(
                    new UserId(externalUserDetails.Id),
                    externalUserDetails.Username,
                    userPricingGroup);

                // Add to dictionary
                availableSharpUsers.Add(sharpUser.Id, sharpUser);

                // Persist user in sharp db
                await _userRepository.AddAsync(sharpUser);
            }
            else
            {
                // User already exists, update him
                sharpUser.UpdatePricingGroup(userPricingGroup);

                _userRepository.Update(sharpUser);
            }
        }

        if (availableSharpUsers.Count == 0)
            return Error.Unexpected(
                "PrimirestToSharpUserOnboarderService.NoneOnboarded",
                "No sharp users were added after onboarding");

        return availableSharpUsers.Values.ToList();
    }
}