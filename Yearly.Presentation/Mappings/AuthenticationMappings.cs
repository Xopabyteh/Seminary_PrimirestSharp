using Mapster;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Contracts.Authentication;

namespace Yearly.Presentation.Mappings;

public class AuthenticationMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoginRequest, LoginQuery>();
        config.NewConfig<LoginResult, LoginResponse>();
    }
}