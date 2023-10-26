//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Yearly.Domain.Models.FoodAgg.ValueObjects;
//using Yearly.Domain.Models.SoupAgg;

//namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

//public class SoupConfiguration : IEntityTypeConfiguration<Soup>
//{
//    public void Configure(EntityTypeBuilder<Soup> builder)
//    {
//        builder.HasKey(s => s.Id);
//        builder
//            .Property(s => s.Id)
//            .HasConversion(
//                id => id.Value,
//                idValue => new FoodId(idValue));

//        builder
//            .Property(s => s.AliasForFoodId)
//            .HasConversion(
//                id => id!.Value,
//                idValue => new FoodId(idValue))
//            .IsRequired(false);

//        builder.OwnsMany(s => s.PhotoIds, photoIdBuilder =>
//        {
//            photoIdBuilder.ToTable("FoodPhotoIds");

//            photoIdBuilder.WithOwner().HasForeignKey(nameof(FoodId));

//            photoIdBuilder.HasKey(p => p.Value);
//            photoIdBuilder
//                .Property(p => p.Value)
//                .HasColumnName("PhotoId")
//                .ValueGeneratedNever();
//        });

//        builder.Metadata
//            .FindNavigation(nameof(Soup.PhotoIds))!
//            .SetPropertyAccessMode(PropertyAccessMode.Field);

//        builder
//            .Property(s => s.Name)
//            .HasMaxLength(256)
//            .IsRequired();
//    }
//}