using Azure.Identity;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.NotificationHubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.BackgroundJobs;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Persistence;
using Yearly.Infrastructure.Persistence.PhotosStorage;
using Yearly.Infrastructure.Persistence.Repositories;
using Yearly.Infrastructure.Persistence.Seeding;
using Yearly.Infrastructure.Services;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Foods;
using Yearly.Infrastructure.Services.Menus;
using Yearly.Infrastructure.Services.Notifications;
using Yearly.Infrastructure.Services.Orders;

namespace Yearly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        //Add Azure key vault if in production
        if (builder.Environment.IsProduction())
        {
            var keyVaultUrl =
                builder.Configuration[
                    "KeyVaultUrl"]
                !; // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUrl),
                new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions {ExcludeSharedTokenCacheCredential = true}));
        }

        services.AddHttpClient(HttpClientNames.Primirest, client =>
        {
            client.BaseAddress = new Uri("https://www.mujprimirest.cz");
        });

        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        //services.AddScoped<PrimirestAuthService>();
        //services.AddScoped<IAuthService>(sp =t6> sp.GetRequiredService<PrimirestAuthService>());

        services.AddScoped<IAuthService, PrimirestAuthService>();
        services.AddScoped<IPrimirestAdminLoggedSessionRunner, PrimirestAdminLoggedSessionRunner>();

        services.AddScoped<IPrimirestMenuPersister, PrimirestMenuPersister>();
        services.AddScoped<IPrimirestMenuProvider, PrimirestMenuProvider>();

        services.AddScoped<IPrimirestOrderService, PrimirestOrderService>();
        services.AddScoped<IFoodSimilarityService, FoodSimilarityService>();
        services.Configure<FoodSimilarityServiceOptions>(
            builder.Configuration.GetSection(FoodSimilarityServiceOptions.SectionName));

        services.Configure<PrimirestAdminCredentialsOptions>(
            builder.Configuration.GetSection(PrimirestAdminCredentialsOptions
                .SectionName)); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...

        services.AddPersistence(builder);
        services.AddBackgroundJobs();
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        if (builder.Environment.IsDevelopment())
        {
            var shouldUseDevMocks = builder.Configuration.GetValue<bool?>("useMock");
            if (shouldUseDevMocks is not null && shouldUseDevMocks.Value == true)
            {
                services.ReplaceServicesWithDevMocks(builder);
            }
        }

        return services;
    }
    public static IHost UseInfrastructure(this IHost app, IWebHostEnvironment environment, IConfiguration config)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        EnsureServiceInitialization(services);
        
        MigrateDb(services);
        SeedCoreData(services);

        return app;
    }

    public static void AddInfrastructureJobs()
    {
        RecurringJob.AddOrUpdate<FireOutboxDomainEventsJob>(
            nameof(FireOutboxDomainEventsJob),
            x => x.ExecuteAsync(),
            @"* * * * *"); //Every minute
    }

    private static void ReplaceServicesWithDevMocks(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped<IPrimirestMenuProvider, PrimirestMenuProviderDev>();
        services.AddScoped<IPrimirestAdminLoggedSessionRunner, PrimirestAdminLoggedSessionRunnerDev>();
        services.AddScoped<IPrimirestOrderService, PrimirestOrderServiceDev>();
        services.AddScoped<IAuthService, AuthServiceDev>();
    }

    private static void AddPersistence(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped<ISessionCache, SessionCache>();
        services.AddDistributedMemoryCache();
        //services.AddStackExchangeRedisCache(c =>
        //{
        //    c.Configuration = builder.Configuration.GetSection("Persistence").GetSection("RedisConnectionString").Value; // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
        //    c.InstanceName = "primirest-sharp";
        //});

        services.Configure<DatabaseConnectionOptions>(
            builder.Configuration.GetSection(DatabaseConnectionOptions
                .SectionName)); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...

        services.AddTransient<SqlConnectionFactory>(); //Dapper

        services.AddDbContext<PrimirestSharpDbContext>((srp, options) =>
        {
            options.UseSqlServer(
                builder.Configuration.GetSection("Persistence").GetSection("DbConnectionString")
                    .Value); // The section must be in appsettings or secrets.json or somewhere where the presentation layer can grab them...
        });

        services.AddScoped<WeeklyMenuRepository>();
        services.AddScoped<IWeeklyMenuRepository>(sp => sp.GetRequiredService<WeeklyMenuRepository>());
        services.AddScoped<FoodRepository>();
        services.AddScoped<IFoodRepository>(sp => sp.GetRequiredService<FoodRepository>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();

        services.AddScoped<AzurePhotoStorage>();
        services.AddScoped<IPhotoStorage, AzurePhotoStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>());

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(
                builder.Configuration["Persistence:AzureStorageConnectionString:blob"]!,
                preferMsi: builder.Environment.IsDevelopment());
        });
        builder.Services.AddScoped<INotificationHubClient>(_ => NotificationHubClient.CreateClientFromConnectionString(
            builder.Configuration.GetSection("NotificationHub")["FullAccessConnectionString"],
            builder.Configuration.GetSection("NotificationHub")["HubName"]));
        builder.Services.AddScoped<IUserNotificationService, AzureUserNotificationService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<DataSeeder>();

    }


    private static void AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddTransient<FireOutboxDomainEventsJob>();
    }

    private static void EnsureServiceInitialization(IServiceProvider services)
    {
        var azureStorage = services.GetRequiredService<AzurePhotoStorage>();
        azureStorage.EnsureContainerExists().Wait();
    }

    private static void MigrateDb(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PrimirestSharpDbContext>();

        var anyPendingMigrations = dbContext.Database.GetPendingMigrations().Any();
        if(anyPendingMigrations)
        {
            dbContext.Database.Migrate();
        }
    }

    private static void SeedCoreData(IServiceProvider services)
    {
        //Init admin user
        var dataSeeder = services.GetRequiredService<DataSeeder>();
        dataSeeder.SeedCoreData();
    }
}