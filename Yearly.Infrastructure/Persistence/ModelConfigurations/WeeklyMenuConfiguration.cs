using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class WeeklyMenuConfiguration : IEntityTypeConfiguration<WeeklyMenu>
{
    public void Configure(EntityTypeBuilder<WeeklyMenu> builder)
    {
        builder.ToTable("WeeklyMenus");

        builder.HasKey(m => m.Id);
        builder
            .Property(m => m.Id)
            .ValueGeneratedNever();

        builder.OwnsMany(m => m.DailyMenus, menuForDayBuilder =>
        {
            menuForDayBuilder.ToTable("DailyMenus");

            menuForDayBuilder.WithOwner().HasForeignKey(nameof(WeeklyMenu.Id));

            menuForDayBuilder.Property(d => d.FoodIds);

            menuForDayBuilder.Property(d => d.Date);
        });
    }        
}