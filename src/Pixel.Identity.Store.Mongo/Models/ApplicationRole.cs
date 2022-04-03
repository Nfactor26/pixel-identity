using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Pixel.Identity.Store.Mongo.Contracts;
using System.Security.Claims;

namespace Pixel.Identity.Store.Mongo.Models
{
    /// <summary>
    /// ApplicationRole with <see cref="ObjectId"/> as the Identifier type
    /// </summary>
    public class ApplicationRole : ApplicationRole<ObjectId>
    {

    }

    /// <summary>
    /// ApplicationRole extends <see cref="IdentityRole{TKey}"/>
    /// </summary>
    public class ApplicationRole<TKey> : IdentityRole<TKey>, IDocument<TKey> where TKey : IEquatable<TKey>
    {
        public int Version { get; set; } = 1;

        public List<IdentityRoleClaim<TKey>> Claims { get; set; } = new();

        /// <summary>
        /// constructor
        /// </summary>
        public ApplicationRole()
        {

        }

        /// <summary>
        /// copnstructor
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        public ApplicationRole(string roleName) : base(roleName)
        {

        }

        /// <summary>
        /// Check if role has specified <see cref="Claim"/>
        /// </summary>
        /// <param name="claim"></param>
        /// <returns>True if Claim exists on Role</returns>
        public virtual bool HasClaim(Claim claim)
        {
            return this.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
        }
    }
}
