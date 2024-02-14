using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Yearly.Queries.DTORepositories;

namespace Yearly.Queries;

public static class DependencyInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<QueryModelOptions>(configuration.GetSection(QueryModelOptions.SectionName));

        services.AddScoped<PhotosDTORepository>();
        services.AddScoped<FoodDTORepository>();
        services.AddScoped<FoodSimilarityTableDTORepository>();
        services.AddScoped<WeeklyMenuDTORepository>();
        services.AddScoped<UserDTORepository>();

        return services;
    }
}