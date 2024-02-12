using Havit.Blazor.Components.Web;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages;

public static class DependencyInjection
{
    public static IServiceCollection AddBlazor(this IServiceCollection services)
    {
        services.AddTransient<BrowserCookieService>();
        services.AddScoped<SessionDetailsService>();

        //services
        //    .AddRazorComponents()
        //    .AddInteractiveServerComponents();

        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddHxServices();
        services.AddHxMessenger();

        return services;
    }
}