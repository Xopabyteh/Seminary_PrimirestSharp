namespace Yearly.MauiClient.Services;

/// <summary>
/// Purely for development and testing. We are using device time anyway
/// </summary>
public class DateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime Today => DateTime.Today;
}