using ErrorOr;

namespace Yearly.Domain.Errors;

internal static class Errors
{
    internal static class Photo
    {
        internal static Error IncorrectAspectRatio
            => Error.Validation("Photo.IncorrectAspectRatio", "The photo must have a 1/1 aspect ratio");

        internal static Error TooSmall(int minSize)
            => Error.Validation("Photo.TooSmall", $"The photo must be at least {minSize}px in side length");
    }
}