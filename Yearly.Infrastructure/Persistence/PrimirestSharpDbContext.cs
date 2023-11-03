using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Infrastructure.Persistence;
public class PrimirestSharpDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrimirestSharpDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

#pragma warning disable CS8618 
    public PrimirestSharpDbContext(DbContextOptions<PrimirestSharpDbContext> options)
#pragma warning restore CS8618 
        : base(options)
    {
    }

    public DbSet<WeeklyMenu> WeeklyMenus { get; set; }
    public DbSet<Food> Foods { get; set; }
    //public DbSet<Soup> Soups { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<User> Users { get; set; }
}
