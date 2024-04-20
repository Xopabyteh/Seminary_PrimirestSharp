using MediatR;
using Yearly.Contracts.Foods;

namespace Yearly.Application.FoodSimilarityTable.Queries;

public record GetFoodSimilarityTableDTOsQuery
    : IRequest<List<FoodSimilarityRecordDTO>>;
