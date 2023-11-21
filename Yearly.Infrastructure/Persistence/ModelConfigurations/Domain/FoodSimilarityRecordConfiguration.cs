using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.ModelConfigurations.Domain;

public class FoodSimilarityRecordConfiguration : IEntityTypeConfiguration<FoodSimilarityRecord>
{
    public void Configure(EntityTypeBuilder<FoodSimilarityRecord> builder)
    {
        builder.ToTable("FoodSimilarities", DatabaseSchemas.Domain);

        builder.HasKey(fs => new { fs.NewlyPersistedFoodId, SecondFoodId = fs.PotentialAliasOriginId });
        builder
            .Property(fs => fs.NewlyPersistedFoodId)
            .HasConversion(
                id => id.Value,
                value => new FoodId(value));
        builder
            .Property(fs => fs.PotentialAliasOriginId)
            .HasConversion(
                id => id.Value,
                value => new FoodId(value));

        builder.Property(fs => fs.Similarity);
    }
}