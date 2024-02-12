using Havit.Blazor.Components.Web;
using Yearly.Presentation.BlazorServer.Services;

namespace Yearly.Presentation.BlazorServer;

public static class DependencyInjection
{
    public static IServiceCollection AddBlazor(this IServiceCollection services)
    {
        services.AddTransient<BrowserCookieService>();

        services
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddHxServices();

        return services;
    }
}