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

    public void Seed()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        SeedUsers();

        _context.SaveChanges();
    }

    private void SeedUsers()
    {
        var adminUser = new User(new UserId(26564871), @"Martin Fiala");
        adminUser.AddRole(UserRole.Admin);
        
        _context.Users.Add(adminUser);
    }
}