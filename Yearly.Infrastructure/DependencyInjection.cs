using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.BackgroundJobs;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Persistence;
using Yearly.Infrastructure.Persistence.OutboxDomainEvents;
using Yearly.Infrastructure.Persistence.PhotosStorage;
using Yearly.Infrastructure.Persistence.Repositories;
using Yearly.Infrastructure.Persistence.Seeding;
using Yearly.Infrastructure.Services;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Foods;
using Yearly.Infrastructure.Services.Menus;
using Yearly.Infrastructure.Services.Orders;
using Yearly.Queries;

namespace Yearly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHttpClient(HttpClientNames.Primirest, client =>
        {
            client.BaseAddress = new Uri("https://www.mujprimirest.cz");
        });

        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IAuthService, PrimirestAuthService>();
        services.AddScoped<PrimirestAuthService>();


        services.AddScoped<IPrimirestMenuProvider, PrimirestMenuProvider>();
        services.AddScoped<IPrimirestOrderService, PrimirestOrderService>();
        services.AddScoped<IFoodSimilarityService, FoodSimilarityService>();

        services.Configure<PrimirestAdminCredentialsOptions>(
            builder.Configuration.GetSection(PrimirestAdminCredentialsOptions.SectionName)); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...

        services.AddPersistence(builder);
        services.AddBackgroundJobs();

        return services;
    }

    private static void AddPersistence(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton<ISessionCache, SessionCache>();
        services.AddDistributedMemoryCache();
        //services.AddStackExchangeRedisCache(c =>
        //{
        //    c.Configuration = builder.Configuration.GetSection("Persistence").GetSection("RedisConnectionString").Value; // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
        //    c.InstanceName = "primirest-sharp";
        //});

        services.Configure<DatabaseConnectionOptions>(
            builder.Configuration.GetSection(DatabaseConnectionOptions.SectionName)); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...

        services.AddTransient<ISqlConnectionFactory, SqlConnectionFactory>(); //Dapper

        services.AddSingleton<DomainEventsToOutboxMessageDatabaseInterceptor>();
        services.AddDbContext<PrimirestSharpDbContext>((srp, options) =>
        {
            var eventsToOutboxInterceptor = srp.GetService<DomainEventsToOutboxMessageDatabaseInterceptor>();

            options.AddInterceptors(eventsToOutboxInterceptor!);

            options.UseSqlServer(
                builder.Configuration.GetSection("Persistence").GetSection("DbConnectionString").Value); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
        });

        services.AddScoped<IWeeklyMenuRepository, WeeklyMenuRepository>();
        services.AddScoped<IFoodRepository, FoodRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        //services.AddScoped<ISoupRepository, SoupRepository>();

        services.AddScoped<IPhotoStorage, AzurePhotoStorage>();
        //services.AddAzureClients(c =>
        //{
        //    c.AddBlobServiceClient(builder.Configuration["Persistence:AzureStorageConnectionString:blob"]);
        //});
        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(builder.Configuration["Persistence:AzureStorageConnectionString:blob"]!, preferMsi: true);
            clientBuilder.AddQueueServiceClient(builder.Configuration["Persistence:AzureStorageConnectionString:queue"]!, preferMsi: true);
        });


        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<DataSeeder>();
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddTransient<FireOutboxDomainEventsJob>();

        return services;
    }
}