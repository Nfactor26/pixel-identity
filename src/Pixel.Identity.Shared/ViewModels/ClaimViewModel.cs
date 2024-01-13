using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace Pixel.Identity.Shared.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="Claim"/> model
    /// </summary>
    [DataContract]
    public class ClaimViewModel
    {
        /// <summary>
        /// Type of Claim
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Value of the Claim
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the Claim should be included in Access token
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public bool IncludeInAccessToken { get; set; } = true;

        /// <summary>
        /// Indicates whether the Claim should be included in Identity token
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public bool IncludeInIdentityToken { get; set; } = false;

        /// <summary>
        /// constructor
        /// </summary>
        public ClaimViewModel()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type">Type of claim</param>
        /// <param name="value">Value of claim</param>
        public ClaimViewModel(string type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Create a <see cref="ClaimViewModel"/> object from given instance of <see cref="Claim"/> 
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static ClaimViewModel FromClaim(Claim claim)
        {
            var claimViewModel = new ClaimViewModel(claim.Type, claim.Value);
            claimViewModel.IncludeInAccessToken = GetPropertyValue(nameof(ClaimViewModel.IncludeInAccessToken));
            claimViewModel.IncludeInIdentityToken = GetPropertyValue(nameof(ClaimViewModel.IncludeInIdentityToken));       
            return claimViewModel;

            bool GetPropertyValue(string propertyName)
            {
                if(claim.Properties.ContainsKey(propertyName))
                {
                    return bool.TryParse(claim.Properties[propertyName], out bool value) && value;                   
                }              
                return false;
            }
        }

        /// <summary>
        /// Create a <see cref="Claim"/> object from current instance of <see cref="ClaimViewModel"/> 
        /// </summary>
        /// <returns></returns>
        public Claim ToClaim()
        {
            var claim = new Claim(this.Type, this.Value);
            claim.Properties.Add(nameof(IncludeInAccessToken), this.IncludeInAccessToken.ToString());
            claim.Properties.Add(nameof(IncludeInIdentityToken), this.IncludeInIdentityToken.ToString());
            return claim;
        }
    }
}
