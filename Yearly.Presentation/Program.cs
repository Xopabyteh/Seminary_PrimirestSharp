using Hangfire;
using Yearly.Application;
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
    //Seed data (before hangfire initializes in the db)
    using var scope = app.Services.CreateScope();
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    //dataSeeder.Seed();
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
}

app.Run();