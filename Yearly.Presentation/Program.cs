using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yearly.Application;
using Yearly.Infrastructure;
using Yearly.Presentation;
using Yearly.Presentation.Errors;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddApplication()
        .AddPresentation()
        .AddInfrastructure();
}


var app = builder.Build();
{

    app.MapControllers();
    app.Run();
}


