using Yearly.Domain.Models;

namespace Yearly.Application.Common.Interfaces;
/// <summary>
/// Responsible for creating a database transaction
/// and storing domain events to the outbox.
/// Will automatically collect domain events from aggregates,
/// or they may be published by other means using <see cref="PublishDomainEvent"/>
/// </summary>
public interface IUnitOfWork
{
    public Task SaveChangesAsync();

    /// <summary>
    /// Ensures that an entity will be updated, when it was not tracked.
    /// For example an entity that is being cached.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    public void AddForUpdate<T>(T entity)
        where T : class;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="domainEvent"></param>
    public void PublishDomainEvent(IDomainEvent domainEvent);
}
