﻿using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Yearly.Infrastructure.Persistence;
using Yearly.Presentation;

namespace Yearly.Application.SubcutaneousTests.Common;

public class WebAppFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    public async Task<IMediator> CreateMediatorAndResetDbAsync()
    {
        var serviceScope = Services.CreateScope();

        await SqlServerTestDatabaseConfigurator.ResetDatabaseAsync();

        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Use in-memory database for testing
            services
                .RemoveAll<DbContextOptions<PrimirestSharpDbContext>>()
                .AddDbContext<PrimirestSharpDbContext>((_, options) =>
                    options.UseSqlServer(SqlServerTestDatabaseConfigurator.ConnectionString));

        });
    }
}