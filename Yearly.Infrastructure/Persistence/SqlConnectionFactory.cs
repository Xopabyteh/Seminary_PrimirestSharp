using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Yearly.Infrastructure.Persistence;

public class SqlConnectionFactory
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