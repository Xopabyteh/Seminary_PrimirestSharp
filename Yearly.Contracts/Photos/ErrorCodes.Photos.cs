namespace Yearly.Contracts;

public static partial class ErrorCodes
{  
    public static class Photo
    {
        public const string PhotoNotFound = "Photos.PhotoNotFound";

        public const string PhotoTooSmall = "Photos.PhotoTooSmall";
        public const string ThumbnailAlreadySet = "Photos.ThumbnailAlreadySet";
    }
}
