using Microsoft.Extensions.DependencyInjection;
using Yearly.Application.Common.Interfaces;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Services;
using Yearly.Infrastructure.Services.Authentication;

namespace Yearly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientNames.Primirest, client =>
        {
            client.BaseAddress = new Uri("https://www.mujprimirest.cz");
        });

        services.AddScoped<IPrimirestAuthService, PrimirestAuthService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}