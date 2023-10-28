using Mapster;
using Yearly.Application.Orders.Commands;
using Yearly.Contracts.Order;

namespace Yearly.Presentation.Mappings;

public class OrderMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(OrderFoodRequest request, string sessionCookie), OrderFoodCommand>()
            .Map(dst => dst.SessionCookie, src => src.sessionCookie)
            .Map(dst => dst.PrimirestFoodIdentifier, src => src.request.PrimirestFoodIdentifier);
    }
}