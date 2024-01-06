using Pixel.Identity.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    [DataContract]
    public class AddClaimRequest
    {
        /// <summary>
        /// User or Role to which claim should be added
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]      
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Claim to add to the role
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel ClaimToAdd { get; set; } = new ClaimViewModel();

        /// <summary>
        /// constructor
        /// </summary>
        public AddClaimRequest()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="claimToAdd"></param>
        public AddClaimRequest(string owner, ClaimViewModel claimToAdd)
        {
            this.Owner = owner;
            this.ClaimToAdd = claimToAdd;
        }
    }
}
