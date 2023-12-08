using Mapster;
using Yearly.Application.Authentication.Commands;
using Yearly.Contracts.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Mappings;

public class AuthenticationMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoginRequest, LoginCommand>();

        config.NewConfig<UserRole, UserRoleDTO>()
            .Map(dst => dst.RoleCode, src => src.RoleCode);

        config.NewConfig<LoginResult, LoginResponse>()
            .Map(dst => dst.SessionCookie, src => src.SessionCookie)
            .Map(dst => dst.UserDetails.Username, src => src.User.Username)
            .Map(dst => dst.UserDetails.Roles, src => src.User.Roles);
    }
}