using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Yearly.Application.Authentication.Commands;
using Yearly.Application.SubcutaneousTests.Common;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services.Authentication;

namespace Yearly.Application.SubcutaneousTests.Authentication.Commands;

[Collection(WebAppFactoryCollection.CollectionName)]
public class LoginCommandTests(WebAppFactory webAppFactory)
{
    [Fact]
    public async Task Login_WithAdminAcc_ReturnsAccountAndSessionCookie()
    {
        // Arrange
        var mediator = await webAppFactory.CreateMediatorAndResetDbAsync();
        var adminCredentials = webAppFactory.Services.GetService<IOptions<PrimirestAdminCredentialsOptions>>();
        var loginCommand = new LoginCommand(adminCredentials!.Value.AdminUsername, adminCredentials.Value.AdminPassword);

        // Act
        var result = await mediator.Send(loginCommand);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.SessionCookie.Should().NotBeNullOrEmpty();
        result.Value.AvailableUsers.Should().NotBeEmpty();
    }
}