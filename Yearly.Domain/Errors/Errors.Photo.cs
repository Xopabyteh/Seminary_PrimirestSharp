using ErrorOr;

namespace Yearly.Domain.Errors;

internal static class Errors
{
    internal static class Photo
    {
        internal static Error TooSmall(int minSize)
            => Error.Validation("Photo.TooSmall", $"The photo must be at least {minSize}px in side length");

        internal static Error ThumbnailAlreadySet()
            => Error.Validation("Photo.ThumbnailAlreadySet", "The thumbnail has already been set");
    }
}