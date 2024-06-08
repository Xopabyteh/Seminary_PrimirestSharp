using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PrimirestSharpDbContext _context;

    public UserRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns the user as tracking
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<User?> GetByIdAsync(UserId id)
    {
        var user = await _context.Users
            .AsTracking()
            .SingleOrDefaultAsync(u => u.Id == id);
        
        return user;
    }

    public Task<bool> DoesUserExistAsync(string username)
    {
        return _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<Dictionary<UserId, User>> GetUsersByIdsAsync(UserId[] ids)
    {
        var users = await _context.Users
            .AsSingleQuery()
            .Where(u => ids.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);
    
        return users ?? new();    
    }
}