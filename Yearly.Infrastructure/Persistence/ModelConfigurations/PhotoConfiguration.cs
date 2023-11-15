using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(p => p.Id);
        builder
            .Property(p => p.Id)
            .ValueGeneratedNever();

        builder
            .Property(p => p.PublisherId)
            .ValueGeneratedNever();

        builder.Property(p => p.PublishDate);

        builder
            .Property(p => p.FoodId)
            .ValueGeneratedNever();

        builder
            .Property(p => p.Link)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.IsApproved);
    }
}