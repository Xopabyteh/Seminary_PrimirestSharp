using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations.Domain;

public class WeeklyMenuConfiguration : IEntityTypeConfiguration<WeeklyMenu>
{
    public void Configure(EntityTypeBuilder<WeeklyMenu> builder)
    {
        builder.ToTable("WeeklyMenus", DatabaseSchemas.Domain);

        builder.HasKey(m => m.Id);
        builder
            .Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                idValue => new WeeklyMenuId(idValue))
            .ValueGeneratedNever();
        //builder.ComplexProperty(m => m.Id);

        builder.OwnsMany(m => m.DailyMenus, dailyMenuBuilder =>
        {
            dailyMenuBuilder.ToTable("DailyMenus");

            dailyMenuBuilder.WithOwner().HasForeignKey(nameof(WeeklyMenuId));

            dailyMenuBuilder.OwnsMany(d => d.Foods, foodIdBuilder =>
            {
                foodIdBuilder.ToTable("MenuFoodIds");

                foodIdBuilder.WithOwner();

                foodIdBuilder.HasKey(f => f.FoodId);
                foodIdBuilder
                    .Property(f => f.FoodId)
                    .HasConversion(
                        id => id.Value,
                        idValue => new FoodId(idValue))
                    .HasColumnName("FoodId")
                    .ValueGeneratedNever();

            });

            dailyMenuBuilder.Property(d => d.Date);
        });
    }
}