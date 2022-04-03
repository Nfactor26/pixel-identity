using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Pixel.Identity.Store.Mongo.Contracts;
using System.Security.Claims;

namespace Pixel.Identity.Store.Mongo.Models
{
    /// <summary>
    /// ApplicationUser with <see cref="ObjectId"/> as the Identifier type
    /// </summary>
    public class ApplicationUser : ApplicationUser<ObjectId, IdentityUserClaim, 
        IdentityUserLogin<ObjectId>, IdentityUserToken<ObjectId>>
    {
       
    }

    /// <summary>
    /// ApplicationUser extends <see cref="IdentityUser{TKey}"/>
    /// </summary>
    public class ApplicationUser<TKey, TUserClaim, TUserLogin, TUserToken> 
        : IdentityUser<TKey> , IDocument<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// version of the document
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Roles mapped to the user
        /// </summary>
        public List<TKey> Roles { get; set; } = new();

        /// <summary>
        /// Claims that belong to user
        /// </summary>
        public List<TUserClaim> Claims { get; set; } = new();

        /// <summary>
        /// External logins belonging to a user 
        /// </summary>
        public List<TUserLogin> Logins { get; set; } = new();

        /// <summary>
        /// Tokens belonging to user e.g. recovery codes and authenticator key
        /// </summary>
        public List<TUserToken> Tokens { get; set; } = new();

        /// <summary>
        /// constructor
        /// </summary>
        public ApplicationUser()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="userName">name of the user</param>
        /// <param name="email">emaild of the user</param>
        public ApplicationUser(string userName, string email) : base(userName)
        {
            this.Email = email;
        }

        /// <summary>
        /// Check if role is mapped to user
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>True if user belongs to specified role</returns>
        public virtual bool HasRole(TKey roleId)
        {
            return this.Roles.Contains(roleId);
        }

        /// <summary>
        /// Check if user has specified <see cref="Claim"/>
        /// </summary>
        /// <param name="claim"></param>
        /// <returns>True if Claim belongs to user</returns>
        public virtual bool HasClaim(Claim claim)
        {
            return this.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
        }

        /// <summary>
        /// Check if user has specified <see cref="UserLoginInfo"/>
        /// </summary>
        /// <param name="userLoginInfo"></param>
        /// <returns>True if UserLoginInfo belongs to user</returns>
        public virtual bool HasLogin(UserLoginInfo userLoginInfo)
        {
            return this.Logins.Any(e => e.LoginProvider == userLoginInfo.LoginProvider && e.ProviderKey == userLoginInfo.ProviderKey);
        }

        /// <summary>
        /// Check if user has specified <see cref="IdentityUserToken{TKey}"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if Token exists for user</returns>
        public virtual bool HasToken(TUserToken token)
        {
            return this.Tokens.Any(t => t.LoginProvider == token.LoginProvider && t.Name == token.Name && t.Value == token.Value);
        }
    }
}
