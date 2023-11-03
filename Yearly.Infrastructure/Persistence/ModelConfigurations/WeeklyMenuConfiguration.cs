using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
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
            .HasConversion(
                id => id.Value,
                idValue => new WeeklyMenuId(idValue))
            .ValueGeneratedNever();

        builder.OwnsMany(m => m.DailyMenus, menuForDayBuilder =>
        {
            menuForDayBuilder.ToTable("DailyMenus");

            menuForDayBuilder.WithOwner().HasForeignKey(nameof(WeeklyMenuId));

            menuForDayBuilder.OwnsMany(d => d.FoodIds, foodIdBuilder =>
            {
                foodIdBuilder.ToTable("MenuFoodIds");

                foodIdBuilder.WithOwner();

                foodIdBuilder.HasKey(f => f.Value);
                foodIdBuilder
                    .Property(f => f.Value)
                    .HasColumnName("FoodId")
                    .ValueGeneratedNever();
            });


            menuForDayBuilder.Property(d => d.Date);
        });
    }        
}