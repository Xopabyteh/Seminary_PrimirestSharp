namespace Yearly.Domain.Models.UserAgg.ValueObjects;

//[Flags]
//public enum UserRole
//{
//    BlackListedFromTakingPhotos = 0b0,
//    User = 0b1,
//    PhotoVerifier = 010,
//    Admin = 0b100
//}

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

    public static readonly UserRole PhotoVerifier = new("PhV");
    public static readonly UserRole Admin = new("Adm");
    public static readonly UserRole BlackListedFromTakingPhotos = new("BFP");

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private UserRole() //For EF core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }
}