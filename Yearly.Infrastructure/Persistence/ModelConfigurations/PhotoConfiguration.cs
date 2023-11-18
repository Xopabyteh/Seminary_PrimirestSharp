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
            .HasOne(p => p.Publisher)
            .WithMany(u => u.Photos);

        builder.Property(p => p.PublishDate);

        builder
            .HasOne(p => p.Food)
            .WithMany(f => f.Photos);

        builder
            .Property(p => p.Link)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.IsApproved);
    }
}