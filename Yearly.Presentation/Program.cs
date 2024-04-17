using Hangfire;
using Yearly.Application;
using Yearly.Infrastructure;
using Yearly.Presentation;
using Yearly.Presentation.BackgroundJobs;
using Yearly.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder)
    .AddQueries(builder.Configuration)
    .AddInfrastructure(builder)
    .AddApplication();

var app = builder.Build();

app.UseInfrastructure(app.Environment, app.Configuration);

app.UseOutputCache();
app.UseHangfireDashboard(options: new DashboardOptions()
{
    AsyncAuthorization = new []
    {
        new PrimirestSharpAdminHangfireDashboardAuthorizationFilter()
    },
    StatsPollingInterval = 120_000 //Poll once per two minutes (to not burn our DB with auth requests)
});

app.UseAntiforgery();
app.UseStaticFiles();

// Add background jobs
{
    Yearly.Infrastructure.DependencyInjection.AddInfrastructureJobs();

    RecurringJob.AddOrUpdate<PersistAvailableMenusJob>(
        "Persist available menus",
        x => x.ExecuteAsync(),
        @"0 8 * * FRI"); //Every friday at 8:00 - https://crontab.guru/#0_8_*_*_FRI
}

app.MapControllers();

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();