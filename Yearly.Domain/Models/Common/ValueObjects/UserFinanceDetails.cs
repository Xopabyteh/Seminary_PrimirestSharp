namespace Yearly.Domain.Models.Common.ValueObjects;

public class UserFinanceDetails : ValueObject
{
    public UserFinanceDetails(MoneyCzechCrowns accountBalance, MoneyCzechCrowns orderedFor)
    {
        AccountBalance = accountBalance;
        OrderedFor = orderedFor;
    }

    public MoneyCzechCrowns AccountBalance { get; set; }
    public MoneyCzechCrowns OrderedFor { get; set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return AccountBalance;
        yield return OrderedFor;
    }
}