using Mapster;
using Yearly.Application.Orders.Commands;
using Yearly.Contracts.Order;
using Yearly.Domain.Models.Common.ValueObjects;

namespace Yearly.Presentation.Mappings;

public class OrderMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(NewOrderRequest Request, string SessionCookie), OrderFoodCommand>()
            .Map(dst => dst.SessionCookie, src => src.SessionCookie)
            .Map(dst => dst.PrimirestFoodIdentifier, src => src.Request.PrimirestFoodIdentifier);

        config.NewConfig<(CancelOrderRequest Request, string SessionCookie), CancelOrderCommand>()
            .Map(dst => dst.SessionCookie, src => src.SessionCookie)
            .Map(dst => dst.PrimirestFoodOrderIdentifier, src => src.Request.PrimirestOrderIdentifier);

        config.NewConfig<Order, OrderDTO>()
            .Map(dst => dst.SharpFoodId, src => src.ForFood.Value)
            .Map(dst => dst.PrimirestOrderData, src => src.PrimirestOrderData);

        config.NewConfig<PrimirestOrderData, PrimirestOrderDataDTO>()
            .Map(dst => dst.PriceCzechCrowns, src => src.Money.Value)
            .Map(dst => dst.PrimirestOrderIdentifier, src => src.PrimirestFoodOrderIdentifier);

        config.NewConfig<PrimirestFoodOrderIdentifier, PrimirestOrderIdentifierDTO>()
            .Map(dst => dst.MenuId, src => src.MenuId)
            .Map(dst => dst.OrderId, src => src.OrderId)
            .Map(dst => dst.OrderItemId, src => src.OrderItemId);
    }
}