using Pixel.Identity.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    [DataContract]
    public class UpdateClaimRequest
    {
        // <summary>
        /// User or Role on which claim details should be updated
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public string Owner { get; set; }

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
        /// <param name="owner"></param>
        /// <param name="claimToAdd"></param>
        public UpdateClaimRequest(string owner, ClaimViewModel original, ClaimViewModel modified)
        {
            this.Owner = owner;
            this.Original = original;
            this.Modified = modified;
        }
    }
}
