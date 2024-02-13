using Dapper;
using Yearly.Contracts.Common;
using Yearly.Contracts.Photos;

namespace Yearly.Queries.DTORepositories;

public class PhotosDTORepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    public PhotosDTORepository(ISqlConnectionFactory connectionFactory)
    {
        this._connectionFactory = connectionFactory;
    }

    public async Task<WaitingPhotosResponse> GetWaitingPhotosAsync()
    {
        var sql = """
                  SELECT TOP 20
                  	P.Id AS Id,
                  	P.ResourceLink AS ResourceLink,
                  	P.ThumbnailResourceLink AS ThumbnailResourceLink,
                  	P.PublishDate AS PublishDate,
                  	F.Name AS FoodName,
                  	U.Username AS PublisherUsername
                  FROM Domain.Photos P
                  JOIN Domain.Foods F ON P.FoodId_Value = F.Id
                  JOIN Domain.Users U ON P.PublisherId_Value = U.Id
                  WHERE P.IsApproved = 0;
                  """;

        await using var connection = _connectionFactory.Create();
        var waitingPhotos = await connection.QueryAsync<PhotoDTO>(sql);

        return new WaitingPhotosResponse(waitingPhotos.ToList());
    }

    public async Task<MyPhotosResponse> GetUsersPhotosAsync(int userId)
    {
        var sql = """
                  SELECT
                  ResourceLink,
                  ThumbnailResourceLink
                  FROM [Domain].[Photos]
                  WHERE PublisherId_Value = @UserId;
                  
                  SELECT COUNT(*)
                  FROM [Domain].[Photos]
                  WHERE PublisherId_Value = @UserId;
                  """;

        await using var connection = _connectionFactory.Create();
        var gridReader = await connection.QueryMultipleAsync(
            sql,
            param: new {UserId = userId});

        var photoLinks = await gridReader.ReadAsync<PhotoLinkDTO>();
        var totalPhotoCount = await gridReader.ReadSingleAsync<int>();

        return new MyPhotosResponse(photoLinks.ToList(), totalPhotoCount);
    }

    public async Task<List<PhotoWithContextDTO>> GetPhotosWithContextAsync(int pageSize, int pageNumber, CancellationToken ctx)
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
                  OFFSET @Page ROWS
                  FETCH NEXT @PageSize ROWS ONLY;
                  """;

        await using var connection = _connectionFactory.Create();
        //var photos = await connection.QueryAsync<PhotoWithContextDTO>(
        //    sql, 
        //    new {Page = pageNumber * 10}); // * 10, bcz 10 is the page size in the query

        var photos = await connection.QueryAsync<PhotoWithContextDTO>(new CommandDefinition(
            sql,
            parameters: new
            {
                Page = pageNumber * pageSize,
                PageSize = pageSize
            },
            cancellationToken: ctx));
        

        return new List<PhotoWithContextDTO>(photos);
    }
    public async Task<int> GetTotalPhotosCountAsync(CancellationToken ctx)
    {
        var sql = """
                  SELECT COUNT(*)
                  FROM [Domain].[Photos];
                  """;

        await using var connection = _connectionFactory.Create();
        var count = await connection.QuerySingleAsync<int>(new CommandDefinition(
            sql,
            cancellationToken: ctx));

        return count;
    }
}