using Microsoft.EntityFrameworkCore;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PrimirestSharpDbContext _context;

    public UnitOfWork(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void AddForUpdate<T>(T entity) 
        where T : class
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.Update(entity);
    }
}