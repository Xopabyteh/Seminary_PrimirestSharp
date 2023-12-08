namespace Yearly.Contracts.Authentication;

public readonly record struct LoginResponse(string SessionCookie, UserDetailsResponse UserDetails);

public readonly record struct UserDetailsResponse(string Username, List<UserRoleDTO> Roles);

public readonly record struct UserRoleDTO(string RoleCode);