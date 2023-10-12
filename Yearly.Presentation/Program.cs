using Yearly.Application;
using Yearly.Infrastructure;
using Yearly.Presentation;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddApplication()
        .AddPresentation()
        .AddInfrastructure(builder);
}


var app = builder.Build();
{

    app.MapControllers();
    app.Run();
}


