namespace Yearly.Contracts.Foods;

public record FoodSimilarityTableResponse(List<FoodSimilarityRecordResponse> Records);

public record FoodSimilarityRecordResponse(
    FoodSimilarityRecordSliceResponse NewlyPersistedFood,
    FoodSimilarityRecordSliceResponse PotentialAlias,
    double Similarity);

public record FoodSimilarityRecordSliceResponse(string Name, Guid Id);