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

    public async Task<MyPhotosResponse> GetUsersPhotos(int userId)
    {
        var sql = """
                  SELECT
                  ResourceLink,
                  ThumbnailResourceLink
                  FROM [PrimirestSharp].[Domain].[Photos]
                  WHERE PublisherId_Value = @UserId;
                  
                  SELECT COUNT(*)
                  FROM [PrimirestSharp].[Domain].[Photos]
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
}