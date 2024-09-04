using System.Reflection;
using Azure.Monitor.OpenTelemetry.Exporter;
using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Yearly.Presentation.BackgroundJobs;
using Yearly.Presentation.Errors;
using Yearly.Presentation.OutputCaching;
using Yearly.Presentation.Pages;

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

        services.OverrideLogging(builder);
        services.AddOpenTelemetry()
            .WithMetrics(c =>
            {
                c.AddAspNetCoreInstrumentation();
                c.AddHttpClientInstrumentation();

                // App insights
                if (builder.Environment.IsProduction())
                {
                    c.AddAzureMonitorMetricExporter(ac =>
                    {
                        ac.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights")!;
                    });
                }
            })
            .WithTracing(c =>
            {
                c.AddAspNetCoreInstrumentation();
                c.AddHttpClientInstrumentation();

                // App insights
                if (builder.Environment.IsProduction())
                {
                    c.AddAzureMonitorTraceExporter(ac =>
                    {
                        ac.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights")!;
                    });
                }
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

    private static IServiceCollection OverrideLogging(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Clear & Add serilog
        builder.Logging.ClearProviders();

        // Serilog Debugging
        if (builder.Environment.IsDevelopment())
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);
        }

        // Configure + App insights
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration);
            
            // App insights
            if (builder.Environment.IsProduction())
            {
                configuration
                    .WriteTo.ApplicationInsights(
                        connectionString: context.Configuration.GetConnectionString("ApplicationInsights")!,
                        TelemetryConverter.Traces);
            }
        });

        return services;
    }
}