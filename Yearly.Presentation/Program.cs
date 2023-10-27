using Yearly.Application;
using Yearly.Infrastructure;
using Yearly.Infrastructure.Persistence.Seeding;
using Yearly.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder);



var app = builder.Build();

app.MapControllers();
app.UseOutputCache();
if (app.Environment.IsDevelopment())
{
    //Seed data
    using var scope = app.Services.CreateScope();
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    //dataSeeder.Seed();
}

app.Run();

//TODO: Change order identifier in API request docs
//TODO: Change order place ids to some constant
//Todo: Menus for weeks should hold some date info
//Todo: Fix docs!