namespace Yearly.Domain.Models.UserAgg.ValueObjects;

public class UserRole : ValueObject
{
    public string RoleCode { get; }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return RoleCode;
    }

    public UserRole(string roleCode)
    {
        RoleCode = roleCode;
    }

    public static readonly UserRole PhotoApprover = new("PhA");
    public static readonly UserRole Admin = new("Adm");
    public static readonly UserRole BlackListedFromTakingPhotos = new("BFP");
    public static bool IsKnown(string code) => ValidRoles.Any(r => r.RoleCode == code);

    private static readonly List<UserRole> ValidRoles = new()
    {
        PhotoApprover,
        Admin,
        BlackListedFromTakingPhotos
    };

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private UserRole() //For EF core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }
}