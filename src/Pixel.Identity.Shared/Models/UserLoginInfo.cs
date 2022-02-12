namespace Pixel.Identity.Shared.Models
{
    /// <summary>
    /// Represents login information and source for a user record.
    /// </summary>
    public class UserLoginInfo
    {        
        public string LoginProvider
        {
            get;
            set;
        }
       
        public string ProviderKey
        {
            get;
            set;
        }

        public string ProviderDisplayName
        {
            get;
            set;
        }

        public UserLoginInfo()
        {            
        }
    }
}
