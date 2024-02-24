namespace Yearly.MauiClient.Services;

/// <summary>
/// Purely for development and testing. We are using device time anyway
/// </summary>
public class DateTimeProvider
{
#if RELEASE
    public DateTime Now => DateTime.Now;
    public DateTime Today => DateTime.Today;
#endif
#if DEBUG
    public DateTime Now => DateTime.Now;
    public DateTime Today => new DateTime(2024, 2, 23);
#endif
}