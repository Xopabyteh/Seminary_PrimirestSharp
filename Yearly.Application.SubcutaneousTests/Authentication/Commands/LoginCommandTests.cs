using Yearly.Application.SubcutaneousTests.Common;

namespace Yearly.Application.SubcutaneousTests.Authentication.Commands;

[Collection(WebAppFactoryCollection.CollectionName)]
public class LoginCommandTests(WebAppFactory webAppFactory)
{
    [Fact]
    public async Task Login_Returns_SessionCooke()
    {
        var mediator = webAppFactory.CreateMediatorAndResetDbAsync();
    }

    [Fact]
    public async Task Login_OnboardsNewUser()
    {

    }

    [Fact]
    public async Task Login_UpdatesExistingUser()
    {

    }
}