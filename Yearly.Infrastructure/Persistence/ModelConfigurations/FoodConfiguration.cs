using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{

    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.HasKey(f => f.Id);
        builder
            .Property(f => f.Id)
            .HasConversion(
                id => id.Value,
                idValue => new FoodId(idValue));

        builder
            .Property(f => f.AliasForFoodId)
            .HasConversion(
                id => id!.Value,
                idValue => new FoodId(idValue))
            .IsRequired(false);

        builder.OwnsMany(m => m.PhotoIds, photoIdBuilder =>
        {
            photoIdBuilder.ToTable("FoodPhotoIds");

            photoIdBuilder.WithOwner().HasForeignKey(nameof(FoodId));

            photoIdBuilder.HasKey(p => p.Value);
            photoIdBuilder
                .Property(p => p.Value)
                .HasColumnName("PhotoId")
                .ValueGeneratedNever();
        });

        builder
            .Property(m => m.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(m => m.Allergens)
            .HasMaxLength(64)
            .IsRequired();
    }
}