using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Yearly.Queries;

namespace Yearly.Infrastructure.Persistence;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly DatabaseConnectionOptions _dbConnectionOptions;

    public SqlConnectionFactory(IOptions<DatabaseConnectionOptions> dbConnectionOptions)
    {
        this._dbConnectionOptions = dbConnectionOptions.Value;
    }

    public SqlConnection Create()
    {
        return new(_dbConnectionOptions.DbConnectionString);
    }
}