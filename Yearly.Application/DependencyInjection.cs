using Microsoft.Extensions.DependencyInjection;

namespace Yearly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}