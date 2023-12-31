﻿using System.Reflection;
using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yearly.Infrastructure.BackgroundJobs;
using Yearly.Presentation.BackgroundJobs;
using Yearly.Presentation.Errors;
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
        services.AddTransient<PersistAvailableMenusJob>();

        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetSection("Persistence").GetSection("DbConnectionString").Value)); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
        services.AddHangfireServer();
    }
}