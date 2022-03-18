using Pixel.Identity.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    public class RemoveClaimRequest
    {
        /// <summary>
        /// User or Role from which claim should be removed
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public string Owner { get; set; }

        /// <summary>
        /// Claim to remove from the role
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
        /// <param name="owner"></param>
        /// <param name="claimToRemove"></param>
        public RemoveClaimRequest(string owner, ClaimViewModel claimToRemove)
        {
            this.Owner = owner;
            this.ClaimToRemove = claimToRemove;
        }
    }
}
