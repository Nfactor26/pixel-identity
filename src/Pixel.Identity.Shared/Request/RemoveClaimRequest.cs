using Pixel.Identity.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    public class RemoveClaimRequest
    {
        /// <summary>
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
        public ClaimViewModel ClaimToRemove { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public RemoveClaimRequest()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimToRemove"></param>
        public RemoveClaimRequest(string roleName, ClaimViewModel claimToRemove)
        {
            this.RoleName = roleName;
            this.ClaimToRemove = claimToRemove;
        }
    }
}
