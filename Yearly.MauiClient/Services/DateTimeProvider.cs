namespace Yearly.MauiClient.Services;

/// <summary>
/// Purely for development and testing. We are using device time anyway
/// </summary>
public class DateTimeProvider
{
    /// <summary>
    /// Relative time, Not UTC
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Relative time, Not UTC
    /// </summary>
    public DateTime Today => DateTime.Today;

    /// <summary>
    /// Now in Czech timezone (UTC+1)
    /// </summary>
    public DateTime CzechNow => DateTime.UtcNow.AddHours(1);
}