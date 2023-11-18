using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.HasKey(f => f.Id);
        builder
            .Property(f => f.Id)
            .ValueGeneratedNever();

        builder
            .Property(f => f.AliasForFoodId)
            .IsRequired(false);

        builder
            .Property(f => f.PhotoIds)
            .HasField("_photoIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        //var navigation = builder.Metadata.FindNavigation("PhotoIds");
        //navigation.SetPropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);


        builder
            .Property(f => f.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(f => f.Allergens)
            .HasMaxLength(64)
            .IsRequired();

        builder.OwnsOne(f => f.PrimirestFoodIdentifier, pIdBuilder =>
        {
            pIdBuilder
                .Property(i => i.DayId)
                .HasColumnName("PrimirestDayId");
            pIdBuilder
                .Property(i => i.ItemId)
                .HasColumnName("PrimirestItemId");
            pIdBuilder
                .Property(i => i.MenuId)
                .HasColumnName("PrimirestMenuId");
        });
    }
}