namespace Yearly.Contracts.Authentication;

public readonly record struct LoginResponse(
    //string SessionCookie,
    SessionCookieDetails SessionCookieDetails,
    UserDetailsResponse UserDetails);

public readonly record struct SessionCookieDetails(string Value, DateTimeOffset ExpirationDate)
{
    public const string Name = "session";
}

public readonly record struct UserDetailsResponse(string Username, List<UserRoleDTO> Roles);

public readonly record struct UserRoleDTO(string RoleCode)
{
    public static readonly UserRoleDTO PhotoApprover = new("PhA");
    public static readonly UserRoleDTO Admin = new("Adm");
    public static readonly UserRoleDTO BlackListedFromTakingPhotos = new("BFP");
}