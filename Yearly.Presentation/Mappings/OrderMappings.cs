using Mapster;
using Yearly.Application.Orders.Commands;
using Yearly.Contracts.Order;

namespace Yearly.Presentation.Mappings;

public class OrderMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<OrderFoodRequest, OrderFoodCommand>()
            .Map(dst => dst.SessionCookie, src => src.SessionCookie)
            .Map(dst => dst.PrimirestOrderIdentifier, src => src.PrimirestOrderIdentifier);
    }
}