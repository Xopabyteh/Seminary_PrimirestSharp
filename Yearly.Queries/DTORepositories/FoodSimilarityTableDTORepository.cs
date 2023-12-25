using Dapper;
using Yearly.Contracts.Foods;

namespace Yearly.Queries.DTORepositories;

public class FoodSimilarityTableDTORepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public FoodSimilarityTableDTORepository(ISqlConnectionFactory connectionFactory)
    {
        this._connectionFactory = connectionFactory;
    }

    public async Task<List<FoodSimilarityRecordDTO>> GetFoodSimilarityTableAsync()
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

        await using var connection = _connectionFactory.Create();
        var foodSimilarityRecords = await connection
            .QueryAsync<FoodSimilarityRecordSliceDTO, FoodSimilarityRecordSliceDTO, double, FoodSimilarityRecordDTO>(
                sql,
                (newlyPersisted, potentialAlias, similarity) =>
                    new FoodSimilarityRecordDTO(newlyPersisted, potentialAlias, similarity),
                splitOn: "Id, Similarity"
            );

        return foodSimilarityRecords.ToList();
    }
}