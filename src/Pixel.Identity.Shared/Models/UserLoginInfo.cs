namespace Pixel.Identity.Shared.Models
{
    /// <summary>
    /// Represents login information and source for a user record.
    /// </summary>
    public class UserLoginInfo
    {        
        public string LoginProvider { get; set; } = string.Empty;
       
        public string ProviderKey { get; set; } = string.Empty;

        public string ProviderDisplayName { get; set; } = string.Empty;
    }
}
