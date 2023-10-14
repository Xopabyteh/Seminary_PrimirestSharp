using Mapster;

namespace Yearly.Presentation.Mappings;

public class MenuMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // config.NewConfig<List<Menu>, MenusThisWeekResponse>()
        //     .Map(dest => dest.Menus, src => src);
        
        // config.NewConfig<Menu, MenuResponse>()
        //     .Map(dest => dest.Date, src => src.Date)
        //     .Map(dst => dst.Foods, src => sr)
    }
}