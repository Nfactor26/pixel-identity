using Pixel.Identity.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    [DataContract]
    public class AddClaimRequest
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
        public ClaimViewModel ClaimToAdd { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public AddClaimRequest()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimToAdd"></param>
        public AddClaimRequest(string roleName, ClaimViewModel claimToAdd)
        {
            this.RoleName = roleName;
            this.ClaimToAdd = claimToAdd;
        }
    }
}
