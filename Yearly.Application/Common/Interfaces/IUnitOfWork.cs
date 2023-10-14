namespace Yearly.Application.Common.Interfaces;
public interface IUnitOfWork
{
    public Task SaveChangesAsync();
}
