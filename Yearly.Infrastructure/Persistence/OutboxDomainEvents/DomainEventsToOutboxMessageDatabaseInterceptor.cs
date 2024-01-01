using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using Yearly.Domain.Models;
using Yearly.Infrastructure.Services;

namespace Yearly.Infrastructure.Persistence.OutboxDomainEvents;

public class DomainEventsToOutboxMessageDatabaseInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public DomainEventsToOutboxMessageDatabaseInterceptor(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var eventPublishers = dbContext.ChangeTracker.Entries<IDomainEventPublisher>();
        
        //Convert events to outbox messages
        //Clear domain events
        //Save outbox messages

        var outboxMessages = eventPublishers
            .Select(entry => entry.Entity)
            .SelectMany(publisher =>
            {
                var events = publisher.GetDomainEvents();
                publisher.ClearDomainEvents();

                return events;
            })
            .Select(e => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = _dateTimeProvider.UtcNow,
                ContentJson = JsonConvert.SerializeObject(e, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All //Save type along with the content
                    //for easier deserialization
                }),
                Type = e.GetType().Name
            })
            .ToList();

        dbContext
            .Set<OutboxMessage>()
            .AddRange(outboxMessages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}