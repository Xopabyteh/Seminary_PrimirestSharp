using ErrorOr;

namespace Yearly.Infrastructure.Errors;

public static partial class Errors
{
    public static class System
    {
        public static Error InvalidAdminCredentials
            => Error.Validation("System.Credentials", "The admin credentials are incorrect");
    }
}