using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Yearly.Domain.Models.FoodAgg;
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
            .ValueGeneratedNever();

        builder.OwnsMany(m => m.DailyMenus, menuForDayBuilder =>
        {
            menuForDayBuilder.ToTable("DailyMenus");

            menuForDayBuilder.WithOwner().HasForeignKey(nameof(WeeklyMenu.Id));
            menuForDayBuilder.HasKey(d => d.Date);

            //Fill the foods navigation (we can't use owns-many and we can't use has-many)
            //todo: this doesn't work asoinidsn
            menuForDayBuilder.Navigation(d => d.Foods)
                .HasField("_foods")
                .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction)
                .IsRequired();


            menuForDayBuilder.Property(d => d.Date);
        });
    }
}