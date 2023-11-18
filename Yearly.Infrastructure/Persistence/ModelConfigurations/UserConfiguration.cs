using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder
            .Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsMany(u => u.Roles, roleBuilder =>
        {
            roleBuilder.ToTable("UserRoles");

            roleBuilder.WithOwner().HasForeignKey(nameof(User.Id));

            roleBuilder.HasKey(r => r.RoleCode);
            roleBuilder
                .Property(r => r.RoleCode)
                .HasMaxLength(3);
        });

        builder.Property(u => u.PhotoIds);

    }
}