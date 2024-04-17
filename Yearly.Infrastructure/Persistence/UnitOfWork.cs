using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models;
using Yearly.Infrastructure.Persistence.OutboxDomainEvents;
using Yearly.Infrastructure.Services;

namespace Yearly.Infrastructure.Persistence;


public class UnitOfWork : IUnitOfWork
{
    private readonly PrimirestSharpDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnitOfWork(PrimirestSharpDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SaveChangesAsync()
    {
        //// Create transaction
        //await using var transaction = await _context.Database.BeginTransactionAsync();
        
        // Convert domain events to outbox messages
        CollectDomainEventsToOutbox();

        // Save changes
        await _context.SaveChangesAsync();
        //await transaction.CommitAsync();
    }

    public void AddForUpdate<T>(T entity) 
        where T : class
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.Update(entity);
    }

    public void PublishDomainEvent(IDomainEvent domainEvent)
    {
        _context
            .Set<OutboxMessage>()
            .Add(MapDomainEventToOutboxMessage(domainEvent));
    }

    private void CollectDomainEventsToOutbox()
    {
        var eventPublishers = _context.ChangeTracker.Entries<IAggregateRoot>();

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
            .Select(MapDomainEventToOutboxMessage)
            .ToList();

        _context
            .Set<OutboxMessage>()
            .AddRange(outboxMessages);
    }

    private OutboxMessage MapDomainEventToOutboxMessage(IDomainEvent domainEvent)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = _dateTimeProvider.UtcNow,
            ContentJson = JsonConvert.SerializeObject(
                domainEvent,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All //Save type along with the content
                    //for easier deserialization
                }),
            Type = domainEvent.GetType()
                .Name,
            ProcessedOnUtc = null
        };
    }
}