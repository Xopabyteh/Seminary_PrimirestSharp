namespace Yearly.Infrastructure.Errors;

public class InvalidAdminCredentialsException : Exception
{ 
    public InvalidAdminCredentialsException(string message)
        : base(message)
    {

    }
}