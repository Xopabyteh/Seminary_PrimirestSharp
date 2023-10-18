using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(m => m.Id);
        builder
            .Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                idValue => new MenuId(idValue));

        builder.OwnsMany(m => m.FoodIds, foodIdBuilder =>
        {
            foodIdBuilder.ToTable("MenuFoodIds");

            foodIdBuilder.WithOwner().HasForeignKey(nameof(MenuId));

            foodIdBuilder.HasKey(f => f.Value);
            foodIdBuilder
                .Property(f => f.Value)
                .HasColumnName("FoodId")
                .ValueGeneratedNever();
        });
    }
}