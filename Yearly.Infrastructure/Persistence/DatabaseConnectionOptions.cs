namespace Yearly.Infrastructure.Persistence;

public class DatabaseConnectionOptions
{
    public const string SectionName = "Persistence";
    public string DbConnectionString { get; set; } = string.Empty;
}