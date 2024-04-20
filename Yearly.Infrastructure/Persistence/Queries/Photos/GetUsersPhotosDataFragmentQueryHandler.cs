using Dapper;
using MediatR;
using Yearly.Application.Photos.Queries;
using Yearly.Contracts.Common;

namespace Yearly.Infrastructure.Persistence.Queries.Photos;

public class GetUsersPhotosDataFragmentQueryHandler
    : IRequestHandler<GetUsersPhotosDataFragmentQuery, DataFragmentDTO<PhotoLinkDTO>>
{
    private readonly SqlConnectionFactory _connection;

    public GetUsersPhotosDataFragmentQueryHandler(SqlConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<DataFragmentDTO<PhotoLinkDTO>> Handle(GetUsersPhotosDataFragmentQuery request, CancellationToken cancellationToken)
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

        await using var connection = _connection.Create();
        var gridReader = await connection.QueryMultipleAsync(
            sql,
            param: new { UserId = request.UserId.Value });

        var photoLinks = await gridReader.ReadAsync<PhotoLinkDTO>();
        var totalPhotoCount = await gridReader.ReadSingleAsync<int>();

        return new DataFragmentDTO<PhotoLinkDTO>(photoLinks.ToList(), totalPhotoCount);
    }
}