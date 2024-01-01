using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Infrastructure.Persistence.OutboxDomainEvents;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations.Infrastructure;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages",  DatabaseSchemas.Infrastructure);

        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.ContentJson)
            .IsRequired();

        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);
    }
}