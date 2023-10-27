using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Converters;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.MenuForWeekAgg;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class MenuForWeekConfiguration : IEntityTypeConfiguration<MenuForWeek>
{
    public void Configure(EntityTypeBuilder<MenuForWeek> builder)
    {
        builder.ToTable("MenusForWeeks");

        builder.HasKey(m => m.Id);
        builder
            .Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                idValue => new PrimirestMenuForWeekId(idValue))
            .ValueGeneratedNever();

        builder.OwnsMany(m => m.MenusForDays, menuForDayBuilder =>
        {
            menuForDayBuilder.ToTable("MenusForDays");

            menuForDayBuilder.WithOwner().HasForeignKey(nameof(PrimirestMenuForWeekId));

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