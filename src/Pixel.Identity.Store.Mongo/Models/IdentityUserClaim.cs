using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System.Security.Claims;

namespace Pixel.Identity.Store.Mongo.Models
{
    /// <summary>
    /// IdentityUserClaim with <see cref="ObjectId"/> as the Identifier type
    /// </summary>
    public class IdentityUserClaim : IdentityUserClaim<ObjectId>
    {
        /// <summary>
        /// Additional properties associated with this claim
        /// </summary>
        public List<string> Properties { get; set; } = new();

        /// <inheritdoc/>  
        public override Claim ToClaim()
        {
            var claim = base.ToClaim();
            foreach (var permission in this.Properties)
            {
                var keyValue = permission.Split(':');
                claim.Properties.Add(keyValue[0], keyValue[1]);
            }
            return claim;
        }

        /// <inheritdoc/>  
        public override void InitializeFromClaim(Claim claim)
        {
            this.ClaimType = claim.Type;
            this.ClaimValue = claim.Value;
            foreach (var (key, value) in claim.Properties)
            {
                this.Properties.Add($"{key}:{value}");
            }
        }
    }
}
