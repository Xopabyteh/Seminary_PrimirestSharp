using Havit.Blazor.Components.Web;
namespace Yearly.PanelClient;

public static class DependencyInjection
{
    public static IServiceCollection AddPanelClient(this IServiceCollection services)
    {
        services.AddHxServices();
        services.AddHxMessenger();

        return services;
    }
}