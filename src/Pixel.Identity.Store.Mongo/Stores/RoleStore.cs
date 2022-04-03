using Dawn;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using Pixel.Identity.Store.Mongo.Extensions;
using Pixel.Identity.Store.Mongo.Models;
using System.ComponentModel;
using System.Security.Claims;

namespace Pixel.Identity.Store.Mongo.Stores
{
    /// <summary>
    /// Implementation of <see cref="IRoleClaimStore{TRole}"/> used to manage Identity roles and associated claims
    /// </summary>
    /// <typeparam name="TRole">Type representing a <see cref="IdentityRole{TKey}"/></typeparam>
    /// <typeparam name="TRoleClaim">Type representing a <see cref="IdentityRoleClaim{TKey}"/></typeparam>
    /// <typeparam name="TKey">Identifier type for documents e.g. Guid or ObjectId</typeparam>
    public class RoleStore<TRole, TRoleClaim, TKey> : IQueryableRoleStore<TRole>, IRoleClaimStore<TRole>
        where TRole : ApplicationRole<TKey>       
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
       where TKey : IEquatable<TKey>
    {
        private bool disposed;       
        private readonly IMongoCollection<TRole> rolesCollection;

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <inheritdoc/>       
        public IQueryable<TRole> Roles => rolesCollection.AsQueryable();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="rolesCollection"></param>
        /// <param name="describer"></param>
        public RoleStore(IMongoCollection<TRole> rolesCollection, IdentityErrorDescriber describer)
        {
            this.rolesCollection = rolesCollection;
            this.ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }  

        /// <inheritdoc/>  
        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            Guard.Argument(roleId).NotNull().NotEmpty();
            ThrowIfDisposedOrCancelled(cancellationToken);
            return rolesCollection.FindFirstOrDefaultAsync(x => x.Id.Equals(ConvertIdFromString(roleId)), cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>  
        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            Guard.Argument(normalizedRoleName).NotNull().NotEmpty();
            ThrowIfDisposedOrCancelled(cancellationToken);
            return rolesCollection.FindFirstOrDefaultAsync(x => x.NormalizedName == normalizedRoleName, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>  
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            await rolesCollection.InsertOneAsync(role, cancellationToken: cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        /// <inheritdoc/>  
        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var currentConcurrencyStamp = role.ConcurrencyStamp;
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            
            var result = await rolesCollection.ReplaceOneAsync(x => x.Id.Equals(role.Id) && x.ConcurrencyStamp.Equals(currentConcurrencyStamp), role, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged || result.ModifiedCount == 0)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;          
        }

        /// <inheritdoc/>  
        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var result = await rolesCollection.DeleteOneAsync(x => x.Id.Equals(role.Id) && x.ConcurrencyStamp.Equals(role.ConcurrencyStamp), cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged || result.DeletedCount == 0)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        /// <inheritdoc/>  
        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            return Task.FromResult(ConvertIdToString(role.Id));
        }

        /// <inheritdoc/>  
        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            return Task.FromResult(role.Name);
        }

        /// <inheritdoc/>  
        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            return Task.FromResult(role.NormalizedName);
        }

        /// <inheritdoc/>  
        public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (role.NormalizedName != normalizedName)
            {
                role.NormalizedName = normalizedName;
                await rolesCollection.UpdateField<TRole, TKey, string>(role, e => e.NormalizedName, role.NormalizedName);
            }
        }

        /// <inheritdoc/>  
        public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (role.Name != roleName)
            {
                role.Name = roleName;
                await rolesCollection.UpdateField<TRole, TKey, string>(role, e => e.Name, role.Name);
            }
        }

        /// <inheritdoc/>  
        public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            Guard.Argument(role).NotNull();
            Guard.Argument(claim).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if(!role.HasClaim(claim))
            {
                var identityRoleClaim = CreateRoleClaim(role, claim);
                role.Claims.Add(identityRoleClaim);
                await rolesCollection.UpdateOneAsync(x => x.Id.Equals(role.Id), Builders<TRole>.Update.Set(x => x.Claims, role.Claims), cancellationToken: cancellationToken).ConfigureAwait(false);              
            }
            
        }

        /// <inheritdoc/>  
        public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            Guard.Argument(role).NotNull();
            Guard.Argument(claim).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if(role.HasClaim(claim))
            {
                role.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
                await rolesCollection.UpdateOneAsync(x => x.Id.Equals(role.Id), Builders<TRole>.Update.Set(x => x.Claims, role.Claims), cancellationToken: cancellationToken);             
            }
            
        }

        /// <inheritdoc/>  
        public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            Guard.Argument(role).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var dbRole = await rolesCollection.FindFirstOrDefaultAsync(x => x.Id.Equals(role.Id), cancellationToken: cancellationToken).ConfigureAwait(false);
            return dbRole?.Claims.Select(e => e.ToClaim()).ToList() ?? Enumerable.Empty<Claim>().ToList();
        }

        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual TRoleClaim CreateRoleClaim(TRole role, Claim claim)
        {
            var roleClaim = new TRoleClaim();
            roleClaim.InitializeFromClaim(claim);
            return roleClaim;
        }

        #region helper methods

        /// <inheritdoc/>  
        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TKey);
            }

            if (typeof(TKey) == typeof(ObjectId))
            {
                return (TKey)((object)ObjectId.Parse(id));
            }

            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        /// <inheritdoc/>  
        public virtual string ConvertIdToString(TKey id)
        {
            if (id.Equals(default(TKey)))
            {
                return null;
            }
            return id.ToString();
        }


        /// <summary>
        /// Throws if this class has been disposed or cancellation requested on CancellationToken .
        /// </summary>
        protected void ThrowIfDisposedOrCancelled(CancellationToken cancellationToken)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose() => disposed = true;

        #endregion helper methods

    }
}
