namespace Yearly.Queries.DTORepositories;

public class UserDTORepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    public UserDTORepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
}