using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yearly.Presentation.BackgroundServices;
using Yearly.Presentation.Errors;
using Yearly.Presentation.OutputCaching;

namespace Yearly.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings();

        services
            .AddControllers()
            .AddNewtonsoftJson();

        services.AddSingleton<ProblemDetailsFactory, YearlyProblemDetailsFactory>();

        services.AddOutputCaching();

        services.AddLogging(b =>
        {
            b.AddSimpleConsole();
        });

        services.AddHostedService<PersistAvailableMenusBackgroundService>();

        return services;
    }

    private static void AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }

    private static void AddOutputCaching(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddPolicy(OutputCachePolicyName.GetAvailableMenus, policy =>
            {
                policy.Tag(OutputCacheTagName.GetAvailableMenusTag);
            });
        });

    }
}