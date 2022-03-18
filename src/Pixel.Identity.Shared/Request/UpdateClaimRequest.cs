using Pixel.Identity.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    [DataContract]
    public class UpdateClaimRequest
    {// <summary>
        /// Role to which claim should be added
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public string RoleName { get; set; }

        /// <summary>
        /// Claim to add to the role
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel Original { get; set; }

        /// <summary>
        /// Claim to add to the role
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel Modified { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public UpdateClaimRequest()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimToAdd"></param>
        public UpdateClaimRequest(string roleName, ClaimViewModel original, ClaimViewModel modified)
        {
            this.RoleName = roleName;
            this.Original = original;
            this.Modified = modified;
        }
    }
}
