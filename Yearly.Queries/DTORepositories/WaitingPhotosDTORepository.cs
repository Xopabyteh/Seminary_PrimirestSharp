using Microsoft.EntityFrameworkCore;
using Yearly.Contracts.Photos;
using Yearly.Infrastructure.Persistence;

namespace Yearly.Queries.DTORepositories;

public class WaitingPhotosDTORepository
{
    private readonly PrimirestSharpDbContext _context;

    public WaitingPhotosDTORepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task<WaitingPhotosResponse> GetWaitingPhotosAsync()
    {
        var waitingPhotos = await _context
            .Photos
            .Where(p => p.IsApproved == false)
            .Select(p => new PhotoDTO(
                _context.Users.Single(u => u.Id == p.PublisherId).Username,
                p.PublishDate,
                _context.Foods.Single(f => f.Id == p.FoodId).Name,
                p.Link,
                p.Id.Value
            ))
            .ToListAsync();

        return new WaitingPhotosResponse(waitingPhotos);
    }
}