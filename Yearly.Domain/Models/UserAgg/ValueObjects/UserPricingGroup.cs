namespace Yearly.Domain.Models.UserAgg.ValueObjects;

/// <summary>
/// Primirest distinguishes 15+ year old students and less than 15 year old students.
/// </summary>
public enum UserPricingGroup
{
    MoreThan15YearsOldStudent = 0,
    LessThan15YearsOldStudent = 1,
}