using Mapster;

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