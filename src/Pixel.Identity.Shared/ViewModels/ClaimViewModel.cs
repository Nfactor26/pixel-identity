using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace Pixel.Identity.Shared.ViewModels
{
    [DataContract]
    public class ClaimViewModel
    {
        [Required]
        [DataMember(IsRequired = true)]
        public string Type { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string Value { get; set; }     
       
        public ClaimViewModel()
        {

        }

        public ClaimViewModel(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public static ClaimViewModel FromClaim(Claim claim)
        {
            return new ClaimViewModel(claim.Type, claim.Value);              
        }

        public Claim ToClaim()
        {
           return new Claim(this.Type, this.Value);                
        }
    }
}
