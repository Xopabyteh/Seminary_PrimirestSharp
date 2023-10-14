using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg;

public class User : AggregateRoot<UserId>
{
    public string Username { get; private set; }
    
    private readonly List<UserRole> _roles;
    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    private readonly List<PhotoId> _photoIds;
    public IReadOnlyList<PhotoId> PhotoIds => _photoIds.AsReadOnly();

    public User(UserId id, string username)
        : base(id)
    {
        Username = username;
        _roles = new List<UserRole>();
        _photoIds = new List<PhotoId>();
    }
}