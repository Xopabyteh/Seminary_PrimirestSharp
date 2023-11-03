namespace Yearly.Infrastructure.Errors;

/// <summary>
/// Occurs when the api contract of Primirest is not met. Whenever Primirest changes their API, this exception will be thrown.
/// </summary>
public class InvalidPrimirestContractException : Exception
{
    public InvalidPrimirestContractException(string message)
        : base(message)
    {
    }
}