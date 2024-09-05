namespace Yearly.Domain.Models.UserAgg.ValueObjects;

public class UserPricingGroupPredictionOptions
{
    public const string SectionName = "UserPricingGroupPrediction";

    public decimal MoreOrEqual15YearsOldStudent { get; set; }
    public decimal Less15YearsOldStudent { get; set; }
}