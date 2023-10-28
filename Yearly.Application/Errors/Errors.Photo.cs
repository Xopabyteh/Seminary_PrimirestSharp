using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Photo
    {
        public static readonly Error PhotoNotFound
            = Error.NotFound("Photo.PhotoNotFound");
    }
}