using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Errors.Exceptions;
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

    public async Task<User> GetByIdAsync(UserId id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            throw new IllegalStateException($"User with id {id.Value} not found in repository");

        return user;
    }

    public Task<bool> DoesUserExistAsync(string username)
    {
        return _context.Users.AnyAsync(u => u.Username == username);
    }
}