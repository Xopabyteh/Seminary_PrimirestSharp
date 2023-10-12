using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Yearly.Application.Common.Interfaces;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Services;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Menus;

namespace Yearly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHttpClient(HttpClientNames.Primirest, client =>
        {
            client.BaseAddress = new Uri("https://www.mujprimirest.cz");
        });

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IAuthService, PrimirestAuthService>();
        services.AddScoped<PrimirestAuthService>();

        services.AddScoped<IMenuProvider, PrimirestMenuProviderService>();

        services.Configure<PrimirestAdminCredentialsOptions>(
            builder.Configuration.GetSection(PrimirestAdminCredentialsOptions.SectionName)); //The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...

        return services;
    }
}