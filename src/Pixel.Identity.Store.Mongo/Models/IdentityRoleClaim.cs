using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System.Security.Claims;

namespace Pixel.Identity.Store.Mongo.Models
{
    /// <summary>
    /// IdentityRoleClaim with <see cref="ObjectId"/> as the Identifier type
    /// </summary>
    public class IdentityRoleClaim : IdentityRoleClaim<ObjectId>
    {
        /// <summary>
        /// Additional properties associated with this claim
        /// </summary>
        public List<string> Properties { get; set; } = new();

        /// <inheritdoc/>  
        public override Claim ToClaim()
        {
            var claim = base.ToClaim();
            foreach (var property in this.Properties)
            {
                var keyValue = property.Split(':');
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
