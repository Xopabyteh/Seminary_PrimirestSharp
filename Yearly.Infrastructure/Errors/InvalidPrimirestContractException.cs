namespace Yearly.Infrastructure.Errors;

/// <summary>
/// Occurs when the api contract of Primirest is not met
/// </summary>
public class InvalidPrimirestContractException : Exception
{
    public InvalidPrimirestContractException(string message)
        : base(message)
    {
    }
}