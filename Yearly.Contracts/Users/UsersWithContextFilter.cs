namespace Yearly.Contracts.Users;

public class UsersWithContextFilter(string usernameFilter)
{
    public string UsernameFilter { get; set; } = usernameFilter;
}