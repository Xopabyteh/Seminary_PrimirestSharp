using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yearly.Presentation.Errors;
using Yearly.Presentation.Validation;

namespace Yearly.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<ProblemDetailsFactory, YearlyProblemDetailsFactory>();

        return services;
    }
}