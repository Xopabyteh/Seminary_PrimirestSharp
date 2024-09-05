using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication;

/// <summary>
/// Raw user info from primirest auth provider
/// </summary>
/// <param name="AdditionalInfo">
/// This hopefully contains the users age in format:
/// "abcdefg; sk. 3 (studenti 15 a více let)" or
/// "abcdefg; sk. 5 (12Let).
/// It is a really really ugly thing that primirest cooked up...
/// </param>
public readonly record struct PrimirestUserInfo(int Id, string Username, string AdditionalInfo)
{
    /// <summary>
    /// Tries to determine the pricing group of the user based on the additional info
    /// </summary>
    public UserPricingGroup DeterminePricingGroup()
    {
        if (AdditionalInfo.Contains("studenti 15 a v"))
            return UserPricingGroup.MoreThan15YearsOldStudent;

        return UserPricingGroup.LessThan15YearsOldStudent;
    }
}