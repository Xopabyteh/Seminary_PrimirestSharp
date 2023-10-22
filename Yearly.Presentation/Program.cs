using Yearly.Application;
using Yearly.Infrastructure;
using Yearly.Infrastructure.Persistence;
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
    //using var scope = app.Services.CreateScope();
    //var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    //dataSeeder.Seed();
}

app.Run();

//TODO: Differentiate between soup and main course