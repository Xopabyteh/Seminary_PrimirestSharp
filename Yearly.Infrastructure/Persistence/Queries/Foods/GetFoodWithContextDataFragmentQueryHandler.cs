using Dapper;
using MediatR;
using Yearly.Application.Foods.Queries;
using Yearly.Contracts.Common;
using Yearly.Contracts.Foods;

namespace Yearly.Infrastructure.Persistence.Queries.Foods;

public class GetFoodWithContextDataFragmentQueryHandler : IRequestHandler<GetFoodWithContextDataFragmentQuery, DataFragmentDTO<FoodWithContextDTO>>
{
    private readonly SqlConnectionFactory _connection;

    public GetFoodWithContextDataFragmentQueryHandler(SqlConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<DataFragmentDTO<FoodWithContextDTO>> Handle(GetFoodWithContextDataFragmentQuery request, CancellationToken cancellationToken)
    {
        var data = await GetData(request, cancellationToken);
        var totalCount = await GetTotalCount(request, cancellationToken);

        return new DataFragmentDTO<FoodWithContextDTO>(data, totalCount);
    }

    private async Task<List<FoodWithContextDTO>> GetData(GetFoodWithContextDataFragmentQuery request, CancellationToken cancellationToken)
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
                PageOffset = request.PageOffset,
                PageSize = request.PageSize,
                NameFilter = request.Filter.NameFilter
            },
            cancellationToken: cancellationToken));

        return foods.ToList();
    }

    private async Task<int> GetTotalCount(GetFoodWithContextDataFragmentQuery request, CancellationToken cancellationToken)
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
                NameFilter = request.Filter.NameFilter
            },
            cancellationToken: cancellationToken));

        return count;
    }
}