using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request;

[DataContract]
public class UpdateRoleNameRequest
{
    // <summary>
    /// User or Role on which claim details should be updated
    /// </summary>
    [Required]
    [DataMember(IsRequired = true)]
    public string RoleId { get; set; }

    /// <summary>
    /// Claim to add to the role
    /// </summary>
    [Required]
    [DataMember(IsRequired = true)]
    public string NewName { get; set; }
}
