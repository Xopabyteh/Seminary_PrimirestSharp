namespace Yearly.Infrastructure.Services;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
    public DateTime CzechNow { get; }
}