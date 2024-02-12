using System.Reflection;
using Hangfire;
using Havit.Blazor.Components.Web;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yearly.Presentation.BackgroundJobs;
using Yearly.Presentation.BlazorServer;
using Yearly.Presentation.Errors;
using Yearly.Presentation.Http;
using Yearly.Presentation.OutputCaching;

namespace Yearly.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, WebApplicationBuilder builder)
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

        services.AddBackgroundJobs(builder);

        services.AddBlazor();

        services.AddHttpClient(HttpClientNames.SharpAPI, client =>
        {
            client.BaseAddress = builder.Environment.IsProduction()
                ? new Uri(builder.Configuration["API:ProdBaseAddress"]!)
                : new Uri(builder.Configuration["API:DevBaseAddress"]!);

        });

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

    private static void AddBackgroundJobs(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Hangfire impl:
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration["Persistence:DbConnectionString"])); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
        
        services.AddHangfireServer();

        // Background jobs:
        services.AddTransient<PersistAvailableMenusJob>();
    }
}