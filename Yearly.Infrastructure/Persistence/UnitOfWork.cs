using Yearly.Application.Common.Interfaces;

namespace Yearly.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PrimirestSharpDbContext _context;

    public UnitOfWork(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}