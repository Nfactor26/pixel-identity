using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    /// <summary>
    /// Request data for retrieving users.
    /// </summary>
    public class GetUsersRequest : PagedDataRequest
    {
        /// <summary>
        /// Retrieve only those users where user name or user email starts with UsersFilter value 
        /// </summary>
        [DataMember(IsRequired = false)]
        public string UsersFilter { get; set; } = string.Empty;       

    }
}
