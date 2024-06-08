using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.Seeding;

public class DataSeeder
{
    private readonly PrimirestSharpDbContext _context;

    public DataSeeder(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seed data necessary for application to run
    /// </summary>
    public void SeedCoreData()
    {
        var adminUser = new User(new UserId(26564871), @"Martin Fiala");
        var admin = Admin.FromUser(adminUser);
        admin.AddRole(UserRole.Admin, adminUser);

        if (!_context.Users.Any(u => u.Id == adminUser.Id))
        {
            _context.Users.Add(adminUser);
        }

        _context.SaveChanges();
    }

#if DEBUG
    /// <summary>
    /// To be used only in development.
    /// Reconstructs DB and seeds core data
    /// </summary>
    public async Task DbResetAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.MigrateAsync();

        SeedCoreData();
    }
#endif
}
