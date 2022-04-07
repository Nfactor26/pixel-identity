namespace Pixel.Identity.Shared
{
    /// <summary>
    /// Claims hold the name of various claims used by Pixel Identity to define it's authorization
    /// policies
    /// </summary>
    public static class Claims  
    {
        public const string ReadOnlyClaim = "identity_read_only";
        public const string ReadWriteClaim = "identity_read_write";        
    }
}
