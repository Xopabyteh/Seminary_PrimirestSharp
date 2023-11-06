namespace Yearly.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime CzechNow => DateTime.UtcNow.AddHours(1);
}