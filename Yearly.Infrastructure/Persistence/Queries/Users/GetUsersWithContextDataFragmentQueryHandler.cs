using Dapper;
using MediatR;
using Yearly.Application.Users.Queries;
using Yearly.Contracts.Authentication;
using Yearly.Contracts.Common;
using Yearly.Contracts.Users;

namespace Yearly.Infrastructure.Persistence.Queries.Users;

public class GetUsersWithContextDataFragmentQueryHandler
    : IRequestHandler<GetUsersWithContextDataFragmentQuery, DataFragmentDTO<UserWithContextDTO>>
{
    private readonly SqlConnectionFactory _connection;

    public GetUsersWithContextDataFragmentQueryHandler(SqlConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<DataFragmentDTO<UserWithContextDTO>> Handle(GetUsersWithContextDataFragmentQuery request, CancellationToken cancellationToken)
    {
        var data = await GetData(request, cancellationToken);
        var totalCount = await GetTotalCount(request, cancellationToken);

        return new DataFragmentDTO<UserWithContextDTO>(data, totalCount);
    }

    private async Task<List<UserWithContextDTO>> GetData(GetUsersWithContextDataFragmentQuery request, CancellationToken cancellationToken)
    {
        var sql = """
                  SELECT
                  	U.Id,
                  	U.Username,
                  	R.RoleCode
                  FROM [Domain].[Users] U
                  LEFT JOIN [Domain].[UserRoles] R ON U.Id = R.UserId
                  WHERE Username LIKE '%' + @Filter + '%'
                  ORDER BY Id
                  OFFSET @PageOffset ROWS
                  FETCH NEXT @PageSize ROWS ONLY;
                  """;

        await using var connection = _connection.Create();
        var userVMs = await connection.QueryAsync<UserWithContextVM>(new CommandDefinition(
            sql,
            parameters: new
            {
                Filter = request.Filter.UsernameFilter,
                PageOffset = request.PageOffset,
                PageSize = request.PageSize
            },
            cancellationToken: cancellationToken));

        //Map VMs to DTOs 
        var userDTOs = userVMs
            .GroupBy(vm => vm.Id)
            .Select(g => new UserWithContextDTO(
                g.Key,
                g.First().Username,
                g
                    .Where(vm => vm.RoleCode is not null)
                    .Select(vm => new UserRoleDTO(vm.RoleCode!))
                    .ToList()))
            .ToList();

        return [..userDTOs];
    }

    private async Task<int> GetTotalCount(GetUsersWithContextDataFragmentQuery request, CancellationToken cancellationToken)
    {
        var sql = """
                  SELECT COUNT(*)
                  FROM [Domain].[Users]
                  WHERE Username LIKE '%' + @Filter + '%'
                  """;

        await using var connection = _connection.Create();

        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            parameters: new { Filter = request.Filter.UsernameFilter },
            cancellationToken: cancellationToken));
    }

    private readonly record struct UserWithContextVM(int Id, string Username, string? RoleCode);
}