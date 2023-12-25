namespace Yearly.Contracts.Foods;

public record FoodSimilarityTableResponse(List<FoodSimilarityRecordDTO> Records);

public record FoodSimilarityRecordDTO(
    FoodSimilarityRecordSliceDTO NewlyPersistedFood,
    FoodSimilarityRecordSliceDTO PotentialAlias,
    double Similarity);

public record FoodSimilarityRecordSliceDTO(Guid Id, string Name);