using Dapper;
using MediatR;
using Yearly.Application.Photos.Queries;
using Yearly.Contracts.Photos;

namespace Yearly.Infrastructure.Persistence.Queries.Photos;

public class GetWaitingPhotoDTOsQueryHandler 
    : IRequestHandler<GetWaitingPhotoDTOsQuery, List<PhotoDTO>>
{
    private readonly SqlConnectionFactory _connection;

    public GetWaitingPhotoDTOsQueryHandler(SqlConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<List<PhotoDTO>> Handle(GetWaitingPhotoDTOsQuery request, CancellationToken cancellationToken)
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
                  """
        ;

        await using var connection = _connection.Create();
        var waitingPhotos = await connection.QueryAsync<PhotoDTO>(sql);

        return waitingPhotos.ToList();
    }
}