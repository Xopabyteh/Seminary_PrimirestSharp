using Hangfire;
using Yearly.Application;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Infrastructure;
using Yearly.Infrastructure.BackgroundJobs;
using Yearly.Infrastructure.Persistence.Seeding;
using Yearly.Presentation;
using Yearly.Presentation.BackgroundJobs;
using Yearly.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder)
    .AddQueries()
    .AddInfrastructure(builder)
    .AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    //Init admin user
    var adminUser = new User(new UserId(26564871), @"Martin Fiala");
    adminUser.AddRole(UserRole.Admin);

    //Seed data (before hangfire initializes in the db)
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    //dataSeeder.DbReset();
    //dataSeeder.SeedAdminUser(adminUser);
    //dataSeeder.SeedSample1(adminUser);
    //dataSeeder.SaveSeed();

    //Add "debug" session to cache (to be more gentle to the primirest api <3)
    var sessionCache = scope.ServiceProvider.GetRequiredService<ISessionCache>();
    sessionCache.AddAsync("debug", adminUser.Id);
}

app.MapControllers();
app.UseOutputCache();
app.UseHangfireDashboard();

//Add background jobs
{
    RecurringJob.AddOrUpdate<PersistAvailableMenusJob>(
        "Persist available menus",
        x => x.ExecuteAsync(),
        @"0 8 * * FRI"); //Every friday at 8:00 - https://crontab.guru/#0_8_*_*_FRI

    RecurringJob.AddOrUpdate<FireOutboxDomainEventsJob>(
        nameof(FireOutboxDomainEventsJob),
        x => x.ExecuteAsync(),
        @"* * * * *"); //Every minute
}


app.Run();