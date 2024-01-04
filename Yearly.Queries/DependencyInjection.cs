using Microsoft.Extensions.DependencyInjection;
using Yearly.Queries.DTORepositories;

namespace Yearly.Queries;

public static class DependencyInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<PhotosDTORepository>();
        services.AddScoped<FoodSimilarityTableDTORepository>();
        services.AddScoped<WeeklyMenuDTORepository>();

        return services;
    }
}