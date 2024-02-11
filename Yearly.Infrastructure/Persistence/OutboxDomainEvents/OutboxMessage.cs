namespace Yearly.Infrastructure.Persistence.OutboxDomainEvents;

public class OutboxMessage
{
    public required Guid Id { get; set; }
    /// <summary>
    /// Underlying type of the event
    /// </summary>
    public required string Type { get; set; }
    /// <summary>
    /// Content of the object serialized in json
    /// </summary>
    public required string ContentJson { get; set; }
    public required DateTime OccurredOnUtc { get; set; }
    /// <summary>
    /// The time this was handled. If null, the event has not been handled yet.
    /// </summary>
    public required DateTime? ProcessedOnUtc { get; set; }
}