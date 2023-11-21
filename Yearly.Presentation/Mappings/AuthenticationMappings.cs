using Mapster;
using Yearly.Application.Authentication.Commands;
using Yearly.Contracts.Authentication;

namespace Yearly.Presentation.Mappings;

public class AuthenticationMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoginRequest, LoginCommand>();
        //config.NewConfig<(User User, LoginResult LoginResult), LoginResponse>()
        //    .Map(dst => dst.SessionCookie, src => src.LoginResult.SessionCookie)
        //    .Map(dst => dst.Username, src => src.User.Username)
        //    .Map(dst => dst.Id, src => src.User.Id.Value);
        config.NewConfig<LoginResult, LoginResponse>()
            .Map(dst => dst.SessionCookie, src => src.SessionCookie)
            .Map(dst => dst.Username, src => src.User.Username);
    }
}