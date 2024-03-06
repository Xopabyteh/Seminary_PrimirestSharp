namespace Yearly.Contracts.Authentication;

public readonly record struct LoginResponse(
    //string SessionCookie,
    SessionCookieDetails SessionCookieDetails,
    UserDetailsResponse UserDetails);

public readonly record struct SessionCookieDetails(string Value, DateTimeOffset ExpirationDate)
{
    /// <summary>
    /// Name of the cookie in the request
    /// </summary>
    public const string Name = "session";
}

public readonly record struct UserDetailsResponse(string Username, int UserId, List<UserRoleDTO> Roles);

public readonly record struct UserRoleDTO(string RoleCode)
{
    public static readonly UserRoleDTO PhotoApprover = new("PhA");
    public static readonly UserRoleDTO Admin = new("Adm");
    public static readonly UserRoleDTO BlackListedFromTakingPhotos = new("BFP");

    public static readonly UserRoleDTO[] AllRoles = new[]
    {
        PhotoApprover,
        Admin,
        BlackListedFromTakingPhotos
    };
}