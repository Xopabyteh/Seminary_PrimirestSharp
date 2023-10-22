namespace Yearly.Domain.Errors.Exceptions;
/// <summary>
/// Raised when a domain rule is broken, resulting in an illegal state
/// </summary>
public class IllegalStateException : Exception
{
    public IllegalStateException(string message)
        : base(message)
    {
    }
}
