using Mapster;
using Yearly.Application.Authentication.Queries.Login;
using Yearly.Contracts.Authentication;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Presentation.Mappings;

public class AuthenticationMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoginRequest, LoginQuery>();
        config.NewConfig<(User User, LoginResult LoginResult), LoginResponse>()
            .Map(dst => dst.SessionCookie, src => src.LoginResult.SessionCookie)
            .Map(dst => dst.Username, src => src.User.Username)
            .Map(dst => dst.Id, src => src.User.Id.Value);
    }
}