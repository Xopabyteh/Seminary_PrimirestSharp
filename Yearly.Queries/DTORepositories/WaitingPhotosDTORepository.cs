using Dapper;
using Yearly.Contracts.Photos;

namespace Yearly.Queries.DTORepositories;

public class WaitingPhotosDTORepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public WaitingPhotosDTORepository(ISqlConnectionFactory connectionFactory)
    {
        this._connectionFactory = connectionFactory;
    }

    public async Task<WaitingPhotosResponse> GetWaitingPhotosAsync()
    {
        var sql = """
                  SELECT 
                  	P.Id AS Id,
                  	P.Link AS Link,
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
}