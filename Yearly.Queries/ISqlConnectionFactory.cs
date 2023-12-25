using Microsoft.Data.SqlClient;

namespace Yearly.Queries;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}