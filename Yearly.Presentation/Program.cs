using Hangfire;
using MediatR;
using Yearly.Application;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Infrastructure;
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
    var admin = Admin.FromUser(adminUser);
    admin.AddRole(UserRole.Admin, adminUser);

    //Seed data (before hangfire initializes in the db)
    //Use seed profile from args
    var seedProfile = builder.Configuration.GetValue<string?>("seedProfile");
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    var wasSeedSuccess = dataSeeder.Seed(seedProfile, adminUser);
    if (!wasSeedSuccess)
        return;

    //Add "debug" session to cache (to be more gentle to the primirest api in development <3)
    var sessionCache = scope.ServiceProvider.GetRequiredService<ISessionCache>();
    await sessionCache.AddAsync("debug", adminUser.Id);
}

app.MapControllers();
app.UseOutputCache();
app.UseHangfireDashboard(options: new DashboardOptions()
{
    AsyncAuthorization = new []
    {
        new PrimirestSharpAdminHangfireDashboardAuthorizationFilter()
    }
});

//Add background jobs
{
    RecurringJob.AddOrUpdate<PersistAvailableMenusJob>(
        "Persist available menus",
        x => x.ExecuteAsync(),
        @"0 8 * * FRI"); //Every friday at 8:00 - https://crontab.guru/#0_8_*_*_FRI

    //This is to be thought about
    //RecurringJob.AddOrUpdate<FireOutboxDomainEventsJob>(
    //    nameof(FireOutboxDomainEventsJob),
    //    x => x.ExecuteAsync(),
    //    @"* * * * *"); //Every minute
}


app.Run();