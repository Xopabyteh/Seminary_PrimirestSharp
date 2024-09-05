using System.Text.Json.Serialization;
using Yearly.Domain.Models.Common.ValueObjects;

namespace Yearly.Domain.Models.UserAgg.ValueObjects;

/// <summary>
/// Primirest distinguishes 15+ year old students and less than 15 year old students.
/// </summary>
public class UserPricingGroup : ValueObject
{
    public UserPricingGroupEnum PricingGroupEnum { get; set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return PricingGroupEnum;
    }

    public MoneyCzechCrowns GetPricePrediction(UserPricingGroupPredictionOptions options)
        => PricingGroupEnum switch
        {
            UserPricingGroupEnum.MoreOrEqual15YearsOldStudent => new MoneyCzechCrowns(options.MoreOrEqual15YearsOldStudent),
            UserPricingGroupEnum.Less15YearsOldStudent => new MoneyCzechCrowns(options.Less15YearsOldStudent),
            _ => throw new ArgumentOutOfRangeException()
        };

    public static UserPricingGroup MoreOrEqual15YearsOldStudent => new(UserPricingGroupEnum.MoreOrEqual15YearsOldStudent);
    public static UserPricingGroup Less15YearsOldStudent => new(UserPricingGroupEnum.Less15YearsOldStudent);
    private UserPricingGroup(UserPricingGroupEnum pricingGroupEnum)
    {
        PricingGroupEnum = pricingGroupEnum;
    }

    [JsonConstructor]
    private UserPricingGroup()
    {
    }

    public enum UserPricingGroupEnum
    {
        MoreOrEqual15YearsOldStudent,
        Less15YearsOldStudent
    }
}