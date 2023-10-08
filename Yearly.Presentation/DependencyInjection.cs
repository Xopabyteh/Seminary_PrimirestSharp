using System.Reflection;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yearly.Presentation.Errors;
using Yearly.Presentation.Validation;

namespace Yearly.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings();

        services.AddControllers();
        services.AddSingleton<ProblemDetailsFactory, YearlyProblemDetailsFactory>();

        return services;
    }

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}