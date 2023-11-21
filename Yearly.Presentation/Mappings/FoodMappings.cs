using Mapster;
using Yearly.Contracts.Foods;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Presentation.Mappings;

public class FoodMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FoodId, Guid>()
            .Map(dst => dst, src => src.Value);

        config.NewConfig<FoodSimilarityRecord, FoodSimilarityRecordDTO>()
            .Map(dst => dst, src => src);
    }
}