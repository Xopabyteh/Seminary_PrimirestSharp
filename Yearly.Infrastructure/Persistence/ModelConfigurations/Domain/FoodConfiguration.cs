using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations.Domain;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable("Foods", DatabaseSchemas.Domain);

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

        builder.OwnsMany(f => f.PhotoIds, photoIdBuilder =>
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
            .Property(f => f.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(f => f.Allergens)
            .HasMaxLength(64)
            .IsRequired();

        //builder.OwnsOne(f => f.PrimirestFoodIdentifier, pIdBuilder =>
        //{
        //    pIdBuilder.WithOwner().HasForeignKey(nameof(FoodId));

        //    pIdBuilder
        //        .Property(i => i.DayId)
        //        .HasColumnName("PrimirestDayId");
        //    pIdBuilder
        //        .Property(i => i.ItemId)
        //        .HasColumnName("PrimirestItemId");
        //    pIdBuilder
        //        .Property(i => i.MenuId)
        //        .HasColumnName("PrimirestMenuId");
        //});

        builder.ComplexProperty(f => f.PrimirestFoodIdentifier);
    }
}