namespace Yearly.Infrastructure.Persistence.OutboxDomainEvents;

public class OutboxMessage
{
    public Guid Id { get; set; }
    /// <summary>
    /// Underlying type of the event
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Content of the object serialized in json
    /// </summary>
    public string ContentJson { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    /// <summary>
    /// The time this was handled. If null, the event has not been handled yet.
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }
}