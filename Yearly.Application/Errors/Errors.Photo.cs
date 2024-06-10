using ErrorOr;
using Yearly.Contracts;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Photo
    {
        public static readonly Error PhotoNotFound
            = Error.NotFound(ErrorCodes.Photo.PhotoNotFound);
    }
}