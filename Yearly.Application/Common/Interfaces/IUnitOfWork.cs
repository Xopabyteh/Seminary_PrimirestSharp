namespace Yearly.Application.Common.Interfaces;
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
}
