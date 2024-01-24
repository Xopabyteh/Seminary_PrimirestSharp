using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Domain.Errors.Exceptions;
public class RejectingApprovedPhotoException : Exception
{
    public RejectingApprovedPhotoException(
        User rejectingUser,
        Photo approvedPhoto)       
        : base($"{rejectingUser.Id.Value} attempted to reject the photo {approvedPhoto.Id.Value}, which is already approved")
    {
    }
}
