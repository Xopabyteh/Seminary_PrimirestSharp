using Microsoft.EntityFrameworkCore;
using Yearly.Infrastructure.Persistence;

namespace Yearly.Application.SubcutaneousTests.Common;

public static class SqlServerTestDatabaseConfigurator
{
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDb;Database=PrimirestSharpSubCutTestDb;Integrated Security=true;TrustServerCertificate=true;";
    public static async Task ResetDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<PrimirestSharpDbContext>()
            .UseSqlServer(ConnectionString)
        .Options;

        await using var context = new PrimirestSharpDbContext(options);
        // Delete all data from all tables (except migrations history)
        await context.Database.ExecuteSqlRawAsync("""
                                                  -- Disable all foreign key constraints
                                                  DECLARE @disableConstraints NVARCHAR(MAX) = '';
                                                  
                                                  SELECT @disableConstraints += 'ALTER TABLE ' + QUOTENAME(t.name) + ' NOCHECK CONSTRAINT ALL; '
                                                  FROM sys.tables t
                                                  WHERE t.name <> '__EFMigrationsHistory';
                                                  
                                                  EXEC sp_executesql @disableConstraints;
                                                  
                                                  -- Generate DELETE statements
                                                  DECLARE @deleteSql NVARCHAR(MAX) = '';
                                                  
                                                  SELECT @deleteSql += 'DELETE FROM ' + QUOTENAME(t.name) + '; '
                                                  FROM sys.tables t
                                                  WHERE t.name <> '__EFMigrationsHistory';
                                                  
                                                  -- Execute DELETE statements
                                                  EXEC sp_executesql @deleteSql;
                                                  
                                                  -- Re-enable all foreign key constraints
                                                  DECLARE @enableConstraints NVARCHAR(MAX) = '';
                                                  
                                                  SELECT @enableConstraints += 'ALTER TABLE ' + QUOTENAME(t.name) + ' CHECK CONSTRAINT ALL; '
                                                  FROM sys.tables t
                                                  WHERE t.name <> '__EFMigrationsHistory';
                                                  
                                                  EXEC sp_executesql @enableConstraints;
                                                  """);

    }
}