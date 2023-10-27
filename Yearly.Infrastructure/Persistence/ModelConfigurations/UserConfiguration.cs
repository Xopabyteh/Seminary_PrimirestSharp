using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder
            .Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                idValue => new PrimirestUserId(idValue));

        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsMany(u => u.Roles, roleBuilder =>
            {
                roleBuilder.ToTable("UserRoles");

                roleBuilder.WithOwner().HasForeignKey(nameof(PrimirestUserId));

                roleBuilder
                    .Property(r => r.RoleCode)
                    .HasMaxLength(3);
            });


        builder.OwnsMany(u => u.PhotoIds, photoIdBuilder =>
        {
            photoIdBuilder.ToTable("UserPhotoIds");

            photoIdBuilder.WithOwner().HasForeignKey(nameof(PrimirestUserId));
        });
    }
}