using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Persistence;
using Yearly.Infrastructure.Persistence.Repositories;
using Yearly.Infrastructure.Persistence.Seeding;
using Yearly.Infrastructure.Services;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Menus;
using Yearly.Infrastructure.Services.Orders;

namespace Yearly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHttpClient(HttpClientNames.Primirest, client =>
        {
            client.BaseAddress = new Uri("https://www.mujprimirest.cz");
        });

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IAuthService, PrimirestAuthService>();
        services.AddScoped<PrimirestAuthService>();

        services.AddScoped<IPrimirestMenuProvider, PrimirestMenuProvider>();
        services.AddScoped<IPrimirestOrderService, PrimirestOrderService>();

        services.Configure<PrimirestAdminCredentialsOptions>(
            builder.Configuration.GetSection(PrimirestAdminCredentialsOptions.SectionName)); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...

        services.Configure<SharpAdminUserIdsOptions>(
            builder.Configuration.GetSection(SharpAdminUserIdsOptions.SectionName)); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...

        services.AddPersistence(builder);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDbContext<PrimirestSharpDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetSection("Persistence").GetSection("DbConnectionString").Value); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
        });

        services.AddScoped<IMenuForWeekRepository, MenuForWeekRepository>();
        services.AddScoped<IFoodRepository, FoodRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        //services.AddScoped<ISoupRepository, SoupRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<DataSeeder>();

        return services;
    }
}