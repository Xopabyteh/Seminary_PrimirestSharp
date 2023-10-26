using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(p => p.Id);
        builder
            .Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                idValue => new PhotoId(idValue));

        builder
            .Property(p => p.PublisherId)
            .HasConversion(
                id => id.Value,
                idValue => new UserId(idValue))
            .ValueGeneratedNever();

        builder.Property(p => p.PublishDate);

        builder
            .Property(p => p.FoodId)
            .HasConversion(
                id => id.Value,
                idValue => new FoodId(idValue))
            .ValueGeneratedNever();

        builder
            .Property(p => p.Link)
            .HasMaxLength(300)
            .IsRequired();
    }
}