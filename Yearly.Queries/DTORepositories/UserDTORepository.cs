using Dapper;
using Yearly.Contracts.Authentication;

namespace Yearly.Queries.DTORepositories;

public class UserDTORepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    public UserDTORepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<UserWithContextDTO>> GetUsersWithContextAsync(
        UsersWithContextFilter filter,
        int pageOffset,
        int pageSize,
        CancellationToken ctx)
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

        await using var connection = _connectionFactory.Create();
        var userVMs = await connection.QueryAsync<UserWithContextVM>(new CommandDefinition(
            sql,
            parameters: new {
                Filter = filter.UsernameFilter,
                PageOffset = pageOffset,
                PageSize = pageSize
            },
            cancellationToken: ctx));

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

        return new List<UserWithContextDTO>(userDTOs);
    }

    public async Task<int> GetTotalUsersCountAsync(UsersWithContextFilter filter, CancellationToken ctx)
    {
        var sql = """
                  SELECT COUNT(*)
                  FROM [Domain].[Users]
                  WHERE Username LIKE '%' + @Filter + '%'
                  """;

        await using var connection = _connectionFactory.Create();

        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            parameters: new { Filter = filter.UsernameFilter },
            cancellationToken: ctx));
    }

    public class UsersWithContextFilter(string usernameFilter)
    {
        public string UsernameFilter { get; set; } = usernameFilter;
    }

    public readonly record struct UserWithContextVM(int Id, string Username, string? RoleCode);
}