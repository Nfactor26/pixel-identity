using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    /// <summary>
    /// Request data for retrieving roles. 
    /// Request can specify to filter the retrieved roles for a user by name or email.
    /// </summary>
    [DataContract]
    public class GetRolesRequest : PagedDataRequest
    {
        /// <summary>
        /// Retrieve only those roles that start with RoleFilter 
        /// </summary>
        [DataMember(IsRequired = true)]
        public string RoleFilter { get; set; } = string.Empty;
    }
}
