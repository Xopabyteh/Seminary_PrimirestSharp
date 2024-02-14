using Dapper;
using Yearly.Contracts.Foods;

namespace Yearly.Queries.DTORepositories;

public class FoodDTORepository
{
    private readonly ISqlConnectionFactory _connection;

    public FoodDTORepository(ISqlConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<List<FoodWithContextDTO>> GetFoodsWithContextAsync(
        FoodsWithContextFilter filter,
        int pageOffset,
        int pageSize,
        CancellationToken ctx)
    {
        var sql = """
                  SELECT
                    F.Id,
                  	F.Name AS Name,
                  	F.AliasForFoodId,
                  	FO.Name AS AliasOriginName
                  FROM [Domain].[Foods] F
                  LEFT JOIN [Domain].[Foods] FO ON F.AliasForFoodId=FO.Id
                  WHERE F.Name LIKE '%' + @NameFilter + '%'
                  ORDER BY Id
                  OFFSET @PageOffset ROWS
                  FETCH NEXT @PageSize ROWS ONLY;
                  """;

        var connection = _connection.Create();
        var foods = await connection.QueryAsync<FoodWithContextDTO>(new CommandDefinition(
            sql,
            parameters: new
            {
                PageOffset = pageOffset,
                PageSize = pageSize,
                NameFilter = filter.NameFilter
            },
            cancellationToken: ctx));

        return foods.ToList();
    }

    public async Task<int> GetTotalFoodsCountAsync(
        FoodsWithContextFilter filter,
        CancellationToken ctx)
    {
        var sql = """
                  SELECT COUNT(*)
                  FROM [Domain].[Foods]
                  WHERE Name LIKE '%' + @NameFilter + '%';
                  """;

        var connection = _connection.Create();
        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            parameters: new
            {
                NameFilter = filter.NameFilter
            },
            cancellationToken: ctx));

        return count;
    }

    public class FoodsWithContextFilter(string nameFilter)
    {
        public string NameFilter { get; set; } = nameFilter;
    }
}