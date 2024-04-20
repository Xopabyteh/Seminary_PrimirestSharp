using Dapper;
using MediatR;
using Yearly.Application.Photos.Queries;
using Yearly.Contracts.Common;
using Yearly.Contracts.Photos;

namespace Yearly.Infrastructure.Persistence.Queries.Photos;

public class GetPhotosWithContextDataFragmentQueryHandler
    : IRequestHandler<GetPhotosWithContextDataFragmentQuery, DataFragmentDTO<PhotoWithContextDTO>>
{
    private readonly SqlConnectionFactory _connection;

    public GetPhotosWithContextDataFragmentQueryHandler(SqlConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<DataFragmentDTO<PhotoWithContextDTO>> Handle(GetPhotosWithContextDataFragmentQuery request, CancellationToken cancellationToken)
    {
        var data = await GetData(request, cancellationToken);
        var totalCount = await GetTotalCount(request, cancellationToken);

        return new DataFragmentDTO<PhotoWithContextDTO>(data, totalCount);
    }

    private async Task<List<PhotoWithContextDTO>> GetData(GetPhotosWithContextDataFragmentQuery request, CancellationToken cancellationToken)
    {
        var sql = """
                  SELECT
                  	P.Id AS Id,
                  	P.IsApproved AS IsApproved,
                  	P.PublishDate AS PublishDate,
                  	P.ThumbnailResourceLink AS ThumbnailResourceLink,
                  	F.Name AS FoodName,
                  	U.Username AS PublisherUsername
                  FROM [Domain].[Photos] P
                  JOIN [Domain].[Foods] F ON P.FoodId_Value = F.Id
                  JOIN [Domain].[Users] U ON P.PublisherId_Value = U.Id
                  ORDER BY P.PublishDate
                  OFFSET @PageOffset ROWS
                  FETCH NEXT @PageSize ROWS ONLY;
                  """;

        await using var connection = _connection.Create();
        //var photos = await connection.QueryAsync<PhotoWithContextDTO>(
        //    sql, 
        //    new {Page = pageNumber * 10}); // * 10, bcz 10 is the page size in the query

        var photos = await connection.QueryAsync<PhotoWithContextDTO>(new CommandDefinition(
            sql,
            parameters: new
            {
                PageOffset = request.PageOffset,
                PageSize = request.PageSize
            },
            cancellationToken: cancellationToken));

        return photos.ToList();
    }

    private async Task<int> GetTotalCount(GetPhotosWithContextDataFragmentQuery request, CancellationToken cancellationToken)
    {
        var sql = """
                  SELECT COUNT(*)
                  FROM [Domain].[Photos];
                  """;

        await using var connection = _connection.Create();
        var count = await connection.QuerySingleAsync<int>(new CommandDefinition(
            sql,
            cancellationToken: cancellationToken));

        return count;
    }
}