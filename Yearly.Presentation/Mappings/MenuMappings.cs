using Mapster;
using Yearly.Contracts.Menu;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Presentation.Mappings;

public class MenuMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.NewConfig<(List<Menu> menus, List<Food> foods), AvailableMenusResponse>()
            
        //config.NewConfig<Menu, MenuResponse>()
        //    .Map(dest => dest.Date, src => src.Date)
        //    .Map(dest => dest.Foods, src => );
    }
}