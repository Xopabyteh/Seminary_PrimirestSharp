using Microsoft.EntityFrameworkCore;
using Yearly.Infrastructure.Persistence;
using Yearly.Infrastructure.Persistence.Seeding;

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
                                                  
                                                  SELECT @disableConstraints += 'ALTER TABLE ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) + ' NOCHECK CONSTRAINT ALL; '
                                                  FROM sys.tables t
                                                  JOIN sys.schemas s ON t.schema_id = s.schema_id
                                                  WHERE t.name <> '__EFMigrationsHistory';
                                                  
                                                  EXEC sp_executesql @disableConstraints;
                                                  
                                                  -- Generate DELETE statements
                                                  DECLARE @deleteSql NVARCHAR(MAX) = '';
                                                  
                                                  SELECT @deleteSql += 'DELETE FROM ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) + '; '
                                                  FROM sys.tables t
                                                  JOIN sys.schemas s ON t.schema_id = s.schema_id
                                                  WHERE t.name <> '__EFMigrationsHistory';
                                                  
                                                  -- Execute DELETE statements
                                                  EXEC sp_executesql @deleteSql;
                                                  
                                                  -- Re-enable all foreign key constraints
                                                  DECLARE @enableConstraints NVARCHAR(MAX) = '';
                                                  
                                                  SELECT @enableConstraints += 'ALTER TABLE ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) + ' CHECK CONSTRAINT ALL; '
                                                  FROM sys.tables t
                                                  JOIN sys.schemas s ON t.schema_id = s.schema_id
                                                  WHERE t.name <> '__EFMigrationsHistory';
                                                  
                                                  EXEC sp_executesql @enableConstraints;
                                                  """);

        // Seed core data
        var dataSeeder = new DataSeeder(context);
        dataSeeder.SeedCoreData();
    }
}