using Mapster;
using Yearly.Application.Authentication.Commands;
using Yearly.Contracts.Authentication;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Mappings;

public class AuthenticationMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoginRequest, LoginCommand>();

        config.NewConfig<UserRole, UserRoleDTO>()
            .Map(dst => dst.RoleCode, src => src.RoleCode);

        config.NewConfig<User, UserDetailsResponse>() 
            .Map(dst => dst.Username, src => src.Username)
            .Map(dst => dst.UserId, src => src.Id.Value)
            .Map(dst => dst.Roles, src => src.Roles);

        config.NewConfig<LoginResult, LoginResponse>()
            .Map(
                dst => dst.SessionCookieDetails,
                src => new SessionCookieDetails(src.SessionCookie, src.SessionExpirationTime))
            .Map(dst => dst.UserDetails, src => src.User)
            .Map(dst => dst.UserDetails.UserId, src => src.User.Id.Value);
    }
}