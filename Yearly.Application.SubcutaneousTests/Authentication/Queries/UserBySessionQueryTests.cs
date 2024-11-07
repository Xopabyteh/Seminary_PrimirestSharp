using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Yearly.Application.Authentication.Commands;
using Yearly.Application.Authentication.Queries;
using Yearly.Application.SubcutaneousTests.Common;
using Yearly.Infrastructure.Services.Authentication;

namespace Yearly.Application.SubcutaneousTests.Authentication.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public class UserBySessionQueryTests
{
    /// <summary>
    /// After logging in with valid credentials,
    /// the user should be able to get their own info
    /// </summary>
    [Fact]
    public async Task UserBySession_WithValidSession_ReturnsUser()
    {
        // Arrange
        var webAppFactory = new WebAppFactory();
        var mediator = await webAppFactory.CreateMediatorAndResetDbAsync();
        var adminCredentials = webAppFactory.Services.GetService<IOptions<PrimirestAdminCredentialsOptions>>();
        
        var loginCommand = new LoginCommand(
            adminCredentials!.Value.AdminUsername,
            adminCredentials.Value.AdminPassword,
            PreferredUserInTenant: null);
        var loginResult = await mediator.Send(loginCommand);
        var userBySessionQuery = new UserBySessionQuery(loginResult.Value.SessionCookie);

        // Act
        var result = await mediator.Send(userBySessionQuery);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();

        loginResult.Value.ActiveLoggedUser.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(loginResult.Value.ActiveLoggedUser);
    }
}