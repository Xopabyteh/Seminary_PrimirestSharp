using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models;
using Yearly.Infrastructure.Persistence;
using Yearly.Infrastructure.Persistence.OutboxDomainEvents;
using Yearly.Infrastructure.Services;


namespace Yearly.Infrastructure.BackgroundJobs;

[DisableConcurrentExecution(timeoutInSeconds: 60)]
public class FireOutboxDomainEventsJob
{
    /// <summary>
    /// How many messages can we fire on every execution
    /// </summary>
    private const int k_HandleMessagesAtATime = 20;
    
    private readonly PrimirestSharpDbContext _dbContext;
    private readonly IPublisher _mediator;
    private readonly ILogger<FireOutboxDomainEventsJob> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public FireOutboxDomainEventsJob(
        PrimirestSharpDbContext dbContext,
        IPublisher mediator,
        ILogger<FireOutboxDomainEventsJob> logger,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        
        _dbContext = dbContext;
        _mediator = mediator;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync()
    {
        var outboxMessages = await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)       // Not yet processed
            //.OrderBy(m => m.OccurredOnUtc)              // Youngest first
            .Take(k_HandleMessagesAtATime)                          // Only n at a time
            .ToListAsync();

        foreach (var outboxMessage in outboxMessages)
        {
            outboxMessage.ProcessedOnUtc = _dateTimeProvider.UtcNow;

            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.ContentJson, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });

            if (domainEvent is null)
            {
                _logger.LogError(
                    "The outbox message ( id={i}; type={t};occurredUtc={o}  ) had content = null, so it cannot be converted and published as a domain event, it is flagged as completed and skipped",
                    outboxMessage.Id,
                    outboxMessage.Type,
                    outboxMessage.OccurredOnUtc);
                continue;
            }

            try
            {
                await _mediator.Publish(domainEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Handling of the domain event with the id ({id}) caused an exception of type ( {t} ) with message: ( {m} ). Stack trace: ( {s} )",
                    outboxMessage.Id,
                    e.GetType().Name,
                    e.Message,
                    e.StackTrace);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}