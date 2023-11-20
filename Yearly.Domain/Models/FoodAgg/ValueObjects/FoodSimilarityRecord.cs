namespace Yearly.Domain.Models.FoodAgg.ValueObjects;

/// <summary>
/// Stores the probability, that the food with id <see cref="NewlyPersistedFoodId"/> is an alias of the food with id <see cref="PotentialAliasOriginId"/>.
/// </summary>
public class FoodSimilarityRecord : ValueObject
{
    public FoodId NewlyPersistedFoodId { get; init; }
    public FoodId PotentialAliasOriginId { get; init; }
    public double Similarity { get; init; }

    public FoodSimilarityRecord(FoodId newlyPersistedFoodId, FoodId potentialAliasOriginId, double similarity)
    {
        NewlyPersistedFoodId = newlyPersistedFoodId;
        PotentialAliasOriginId = potentialAliasOriginId;
        Similarity = similarity;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return NewlyPersistedFoodId;
        yield return PotentialAliasOriginId;
        yield return Similarity;
    }
}