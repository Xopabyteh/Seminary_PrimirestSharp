using Dapper;
using MediatR;
using Yearly.Application.FoodSimilarityTable.Queries;
using Yearly.Contracts.Foods;

namespace Yearly.Infrastructure.Persistence.Queries.FoodSimilarityTable;

public class GetFoodSimilarityTableDTOsQueryHandler : IRequestHandler<GetFoodSimilarityTableDTOsQuery, List<FoodSimilarityRecordDTO>>
{
    private readonly SqlConnectionFactory _connection;

    public GetFoodSimilarityTableDTOsQueryHandler(SqlConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<List<FoodSimilarityRecordDTO>> Handle(GetFoodSimilarityTableDTOsQuery request, CancellationToken cancellationToken)
    {
        var sql = """
                  SELECT
                  	NF.Id AS Id,
                  	NF.Name AS Name,
                  	PAOF.Id AS Id,
                  	PAOF.Name AS Name,
                  	S.Similarity
                  FROM Domain.FoodSimilarities S
                  JOIN Domain.Foods NF ON NF.Id = S.NewlyPersistedFoodId
                  JOIN Domain.Foods PAOF ON PAOF.Id = S.PotentialAliasOriginId;
                  """;

        await using var connection = _connection.Create();
        var foodSimilarityRecords = await connection
            .QueryAsync<FoodSimilarityRecordSliceDTO, FoodSimilarityRecordSliceDTO, double, FoodSimilarityRecordDTO>(
                sql,
                (newlyPersisted, potentialAlias, similarity) =>
                    new FoodSimilarityRecordDTO(newlyPersisted, potentialAlias, similarity),
                splitOn: "Id, Similarity");

        return foodSimilarityRecords.ToList();
    }
}