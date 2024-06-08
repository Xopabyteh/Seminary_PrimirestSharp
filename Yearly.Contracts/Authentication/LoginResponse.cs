namespace Yearly.Contracts.Authentication;

/// <param name="SessionCookieDetails"></param>
/// <param name="AvailableUserDetails">
/// List of available users within the primirest "user tenant"
/// to which we can switch contexts
/// </param>
public readonly record struct LoginResponse(
    int InitialActiveUserId,
    UserDetailsResponse[] AvailableUserDetails,
    SessionCookieDetails SessionCookieDetails);

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