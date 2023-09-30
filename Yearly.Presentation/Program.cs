using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yearly.Application;
using Yearly.Application.Services.Authentication;
using Yearly.Contracts;
using Yearly.Infrastructure;
using Yearly.Presentation.Errors;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services
        .AddApplication()
        .AddInfrastructure();

    builder.Services.AddSingleton<ProblemDetailsFactory, YearlyProblemDetailsFactory>();
}


var app = builder.Build();
{

    app.MapControllers();
    app.Run();
}


