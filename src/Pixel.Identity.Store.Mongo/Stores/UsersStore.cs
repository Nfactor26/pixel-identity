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
    /// UserStore is used to manage <see cref="IdentityUser{TKey}"/> which includes roles, claims, logins, tokens , etc.
    /// </summary>
    /// <typeparam name="TUser">Type representing a <see cref="IdentityUser{TKey}"/></typeparam>
    /// <typeparam name="TRole">Type representing a <see cref="IdentityRole{TKey}"/></typeparam>
    /// <typeparam name="TUserClaim">Type representing a <see cref="IdentityUserClaim{TKey}{TKey}"/></typeparam>
    /// <typeparam name="TUserLogin">Type representing a <see cref="IdentityUserLogin{TKey}"/></typeparam>
    /// <typeparam name="TUserToken">Type representing a <see cref="IdentityUserToken{TKey}"/></typeparam>
    /// <typeparam name="TRoleClaim">Type representing a <see cref="IdentityRoleClaim{TKey}"/></typeparam>
    /// <typeparam name="TKey">Identifier type for documents e.g. Guid or ObjectId</typeparam>
    public class UserStore<TUser, TRole, TUserClaim, TUserLogin, TUserToken, TRoleClaim, TKey>
       : UserStoreBase<TUser, TRole, TKey, TUserClaim, TUserLogin, TUserToken,TRoleClaim>
       where TUser : ApplicationUser<TKey, TUserClaim, TUserLogin, TUserToken>
       where TRole : ApplicationRole<TKey>
       where TUserClaim : IdentityUserClaim<TKey>, new()
       where TUserLogin : IdentityUserLogin<TKey>, new()
       where TUserToken : IdentityUserToken<TKey>, new()
       where TRoleClaim : IdentityRoleClaim<TKey>, new()
       where TKey : IEquatable<TKey>
    {
        private static readonly InsertOneOptions InsertOneOptions = new InsertOneOptions();
        private static readonly FindOptions<TUser> FindOptions = new FindOptions<TUser>();
        private static readonly ReplaceOptions ReplaceOptions = new ReplaceOptions();


        private readonly IMongoCollection<TUser> usersCollection;
        private readonly IMongoCollection<TRole> rolesCollection;

        /// <inheritdoc/>  
        public override IQueryable<TUser> Users => usersCollection.AsQueryable();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="rolesCollection"></param>
        /// <param name="describer"></param>
        public UserStore(IMongoCollection<TUser> usersCollection, IMongoCollection<TRole> rolesCollection, IdentityErrorDescriber describer)
            : base(describer)
        {
            this.usersCollection = usersCollection;
            this.rolesCollection = rolesCollection;
        }

        #region IUserClaimStore

        /// <inheritdoc/>  
        public override Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);         
            return Task.FromResult<IList<Claim>>(user.Claims.Select(x => x.ToClaim()).ToList());
        }

        /// <inheritdoc/>
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            Guard.Argument(claims).NotNull().NotEmpty();
            ThrowIfDisposedOrCancelled(cancellationToken);

            foreach (var claim in claims)
            {
                if(!user.HasClaim(claim))
                {
                    user.Claims.Add(CreateUserClaim(user, claim));
                    await usersCollection.UpdateField<TUser, TKey, List<TUserClaim>>(user, u => u.Claims, user.Claims);                  
                }
            }            
        }

        /// <inheritdoc/>
        public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            Guard.Argument(claim).NotNull();
            Guard.Argument(newClaim).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var matchedClaims = user.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList();           
            if(matchedClaims.Any())
            {
                foreach (var matchedClaim in matchedClaims)
                {
                    matchedClaim.ClaimValue = newClaim.Value;
                    matchedClaim.ClaimType = newClaim.Type;
                }
                await usersCollection.UpdateField<TUser, TKey, List<TUserClaim>>(user, u => u.Claims, user.Claims);
            }         
        }

        /// <inheritdoc/>
        public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            Guard.Argument(claims).NotNull().NotEmpty();
            ThrowIfDisposedOrCancelled(cancellationToken);

            foreach (var claim in claims)
            {
                user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            }
            await usersCollection.UpdateField<TUser, TKey, List<TUserClaim>>(user, u => u.Claims, user.Claims);
        }

        /// <inheritdoc/>
        public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            Guard.Argument(claim).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            return (await usersCollection.WhereAsync(u => u.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value), cancellationToken).ConfigureAwait(false)).ToList();
        }

        #endregion IUserClaimStore

        #region IUserLoginStore

        /// <inheritdoc/>
        public override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);           
            return Task.FromResult<IList<UserLoginInfo>>(user.Logins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList());
        }

        /// <inheritdoc/>
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            Guard.Argument(login).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if(!user.HasLogin(login))
            {
                user.Logins.Add(CreateUserLogin(user, login));
                await usersCollection.UpdateField<TUser, TKey, List<TUserLogin>>(user, u => u.Logins, user.Logins);
            }           
        }

        /// <inheritdoc/>
        public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if(user.Logins.Any(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey))
            {
                user.Logins.RemoveAll(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
                await usersCollection.UpdateField<TUser, TKey, List<TUserLogin>>(user, u => u.Logins, user.Logins);
            }           
        }

        #endregion IUserLoginStore

        #region IUserRoleStore

        /// <inheritdoc/>
        public override async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            Guard.Argument(roleName).NotNull().NotEmpty();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var role = await FindRoleAsync(roleName, cancellationToken);
            if (role == null)
            {
                return new List<TUser>();
            }
            var filter = Builders<TUser>.Filter.AnyEq(x => x.Roles, role.Id);
            return (await usersCollection.FindAsync(filter, FindOptions, cancellationToken).ConfigureAwait(true)).ToList();
        }

        /// <inheritdoc/>  
        public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var userDb = await FindByIdAsync(ConvertIdToString(user.Id), cancellationToken).ConfigureAwait(true);
            if (userDb == null)
            {
                return new List<string>();
            }

            var roles = new List<string>();
            foreach (var item in userDb.Roles)
            {
                var dbRole = await rolesCollection.FindFirstOrDefaultAsync(r => r.Id.Equals(item), cancellationToken).ConfigureAwait(true);

                if (dbRole != null)
                {
                    roles.Add(dbRole.Name);
                }
            }
            return roles;
        }

        /// <inheritdoc/>  
        public override async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var dbUser = await FindByIdAsync(ConvertIdToString(user.Id), cancellationToken).ConfigureAwait(true);
            var role = await FindRoleAsync(roleName, cancellationToken).ConfigureAwait(true);
            if (role == null)
            {
                return false;
            }
            return dbUser?.Roles.Contains(role.Id) ?? false;
        }

        /// <inheritdoc/>  
        public override async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            Guard.Argument(roleName).NotNull().NotEmpty().NotWhiteSpace();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var roleEntity = await FindRoleAsync(roleName, cancellationToken)
                ?? throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Role {0} does not exist.", roleName));
            if(!user.HasRole(roleEntity.Id))
            {
                user.Roles.Add(roleEntity.Id);
                await usersCollection.UpdateField<TUser, TKey, List<TKey>>(user, u => u.Roles, user.Roles);               
            }           
        }

        /// <inheritdoc/>  
        public override async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            Guard.Argument(roleName).NotNull().NotEmpty().NotWhiteSpace();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var roleEntity = await FindRoleAsync(roleName, cancellationToken);
            if (roleEntity != null)
            {
                if (user.HasRole(roleEntity.Id))
                {
                    user.Roles.Remove(roleEntity.Id);
                    await usersCollection.UpdateField<TUser, TKey, List<TKey>>(user, u => u.Roles, user.Roles);
                }
                   
            }
        }

        #endregion IUserRoleStore

        #region IUserEmailStore

        /// <inheritdoc/>  
        public override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);
            return usersCollection.FindFirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        /// <inheritdoc/>  
        public override async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.Email != email)
            {
                user.Email = email;
                await usersCollection.UpdateField<TUser, TKey, string>(user, e => e.Email, user.Email);
            }
        }

        /// <inheritdoc/>  
        public override async Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.NormalizedEmail != normalizedEmail)
            {
                user.NormalizedEmail = normalizedEmail;
                await usersCollection.UpdateField<TUser, TKey, string>(user, e => e.NormalizedEmail, user.NormalizedEmail);
            }
        }

        /// <inheritdoc/>  
        public override async Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.EmailConfirmed != confirmed)
            {
                user.EmailConfirmed = confirmed;
                await usersCollection.UpdateField<TUser, TKey, bool>(user, e => e.EmailConfirmed, user.EmailConfirmed);
            }
        }

        #endregion IUserEmailStore            

        #region IUserPhoneNumberStore

        /// <inheritdoc/>  
        public override async Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.PhoneNumber != phoneNumber)
            {
                user.PhoneNumber = phoneNumber;
                await usersCollection.UpdateField<TUser, TKey, string>(user, e => e.PhoneNumber, user.PhoneNumber);
            }
        }

        /// <inheritdoc/>  
        public override async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.PhoneNumberConfirmed != confirmed)
            {
                user.PhoneNumberConfirmed = confirmed;
                await usersCollection.UpdateField<TUser, TKey, bool>(user, e => e.PhoneNumberConfirmed, user.PhoneNumberConfirmed);
            }
        }

        #endregion IUserPhoneNumberStore

        #region IUserPasswordStore

        /// <inheritdoc/>  
        public override async Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.UserName != passwordHash)
            {
                user.PasswordHash = passwordHash;
                await usersCollection.UpdateField<TUser, TKey, string>(user, e => e.PasswordHash, user.PasswordHash);
            }
        }

        #endregion IUserPasswordStore

        #region IUserSecurityStampStore

        /// <inheritdoc/>
        public override async Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.SecurityStamp != stamp)
            {
                user.SecurityStamp = stamp;
                await usersCollection.UpdateField<TUser, TKey, string>(user, e => e.SecurityStamp, user.SecurityStamp);
            }
        }

        #endregion IUserSecurityStampStore

        #region IUserLockoutStore

        /// <inheritdoc/>  
        public override async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.LockoutEnd != lockoutEnd)
            {
                user.LockoutEnd = lockoutEnd;
                await usersCollection.UpdateField<TUser, TKey, DateTimeOffset?>(user, e => e.LockoutEnd, user.LockoutEnd);
            }
        }

        /// <inheritdoc/>  
        public override async Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            user.AccessFailedCount++;
            await usersCollection.UpdateField<TUser, TKey, int>(user, e => e.AccessFailedCount, user.AccessFailedCount);
            return user.AccessFailedCount;
        }

        /// <inheritdoc/>  
        public override async Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if(user.AccessFailedCount != 0)
            {
                user.AccessFailedCount = 0;
                await usersCollection.UpdateField<TUser, TKey, int>(user, e => e.AccessFailedCount, user.AccessFailedCount);
            }                   
        }

        /// <inheritdoc/>  
        public override async Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.LockoutEnabled != enabled)
            {
                user.LockoutEnabled = enabled;
                await usersCollection.UpdateField<TUser, TKey, bool>(user, e => e.LockoutEnabled, user.LockoutEnabled);
            }
        }

        #endregion IUserLockoutStore

        #region IUserTwoFactorStore

        /// <inheritdoc/> 
        public override async Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.TwoFactorEnabled != enabled)
            {
                user.TwoFactorEnabled = enabled;
                await usersCollection.UpdateField<TUser, TKey, bool>(user, e => e.TwoFactorEnabled, user.TwoFactorEnabled);
            }
        }

        #endregion IUserTwoFactorStore

        #region IUserAuthenticationTokenStore

        /// <inheritdoc/> 
        public override async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);         

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            if (token == null)
            {
                token = CreateUserToken(user, loginProvider, name, value);               
                await AddUserTokenAsync(token).ConfigureAwait(false);    
                user.Tokens.Add(token);
            }
            else
            {
                token.Value = value;
                await usersCollection.UpdateField<TUser, TKey, List<TUserToken>>(user, u => u.Tokens, user.Tokens);
            }
        }

        /// <inheritdoc/> 
        public override async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            if (entry != null)
            {            
                await RemoveUserTokenAsync(entry).ConfigureAwait(false);
                user.Tokens.Remove(entry);
            }
        }

        #endregion IUserAuthenticationTokenStore

        #region  IUserStore

        /// <inheritdoc/>  
        public override Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);
            var id = ConvertIdFromString(userId);
            return usersCollection.FindFirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        }

        /// <inheritdoc/>  
        public override async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.UserName != userName)
            {
                user.UserName = userName;
                await usersCollection.UpdateField<TUser, TKey, string>(user, e => e.UserName, user.UserName);
            }
        }

        /// <inheritdoc/>  
        public override async Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (user.NormalizedUserName != normalizedName)
            {
                user.NormalizedUserName = normalizedName;
                await usersCollection.UpdateField<TUser, TKey, string>(user, e => e.NormalizedUserName, user.NormalizedUserName);
            }
        }


        /// <inheritdoc/>  
        public override Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);
            return usersCollection.FindFirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>  
        public async override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            await usersCollection.InsertOneAsync(user, InsertOneOptions, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        /// <inheritdoc/>  
        public async override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var currentConcurrencyStamp = user.ConcurrencyStamp;
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            var result = await usersCollection.ReplaceOneAsync(x => x.Id.Equals(user.Id) && x.ConcurrencyStamp.Equals(currentConcurrencyStamp), user, ReplaceOptions, cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged || result.ModifiedCount == 0)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        /// <inheritdoc/>  
        public async override Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            Guard.Argument(user).NotNull();
            ThrowIfDisposedOrCancelled(cancellationToken);

            var result = await usersCollection.DeleteOneAsync(x => x.Id.Equals(user.Id) && x.ConcurrencyStamp.Equals(user.ConcurrencyStamp), cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged || result.DeletedCount == 0)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        #endregion IUserStore

        #region protected overridden methods

        /// <inheritdoc/>  
        protected override async Task AddUserTokenAsync(TUserToken token)
        {
            var user = await FindUserAsync(token.UserId, cancellationToken: CancellationToken.None);
            if(!user.HasToken(token))
            {               
                await usersCollection.UpdateField<TUser, TKey, List<TUserToken>>(user, u => u.Tokens, user.Tokens);
            }                  
        }

        /// <inheritdoc/>  
        protected override async Task RemoveUserTokenAsync(TUserToken token)
        {
            var user = await FindUserAsync(token.UserId, cancellationToken: CancellationToken.None);
            var tokenToRemove = user.Tokens.FirstOrDefault(t => t.LoginProvider == token.LoginProvider &&
                t.Name == token.Name);
            if(tokenToRemove != null)
            {              
                await usersCollection.UpdateField<TUser, TKey, List<TUserToken>>(user, u => u.Tokens, user.Tokens);
            }       
        }

        /// <inheritdoc/>  
        protected override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return rolesCollection.FindFirstOrDefaultAsync(x => x.NormalizedName == normalizedRoleName, cancellationToken);
        }

        /// <inheritdoc/>  
        protected override Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {          
            return Task.FromResult<TUserToken>(user.Tokens?.FirstOrDefault(x => x.LoginProvider == loginProvider && x.Name == name));
        }

        /// <inheritdoc/>  
        protected override Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);
            return usersCollection.FindFirstOrDefaultAsync(x => x.Id.Equals(userId), cancellationToken);
        }

        /// <inheritdoc/>  
        protected override async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var dbUser = await FindUserAsync(userId, cancellationToken).ConfigureAwait(true);
            return dbUser?.Logins?.FirstOrDefault(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
        }

        /// <inheritdoc/>  
        protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var userWithLogin = await usersCollection.FindFirstOrDefaultAsync(u =>
                u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey), cancellationToken).ConfigureAwait(true);
            return userWithLogin?.Logins.FirstOrDefault(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
        }

        #endregion protected overridden methods

        #region Helper methods       

        /// <inheritdoc/>  
        public override TKey ConvertIdFromString(string id)
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
        public override string ConvertIdToString(TKey id)
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
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
        }

        #endregion Helper methods

    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>   
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public abstract class UserStoreBase<TUser, TRole, TKey, TUserClaim, TUserLogin, TUserToken, TRoleClaim> :
        UserStoreBase<TUser, TKey, TUserClaim, TUserLogin, TUserToken>,
        IUserRoleStore<TUser>
        where TUser : ApplicationUser<TKey, TUserClaim, TUserLogin, TUserToken>
        where TRole : ApplicationRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()        
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStoreBase(IdentityErrorDescriber describer) : base(describer) { }

        /// <summary>
        /// Retrieves all users in the specified role.
        /// </summary>
        /// <param name="normalizedRoleName">The role whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that are in the specified role.
        /// </returns>
        public abstract Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds the given <paramref name="normalizedRoleName"/> to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the role to.</param>
        /// <param name="normalizedRoleName">The role to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public abstract Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes the given <paramref name="normalizedRoleName"/> from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the role from.</param>
        /// <param name="normalizedRoleName">The role to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public abstract Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the roles the specified <paramref name="user"/> is a member of.
        /// </summary>
        /// <param name="user">The user whose roles should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the roles the user is a member of.</returns>
        public abstract Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns a flag indicating if the specified user is a member of the give <paramref name="normalizedRoleName"/>.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="normalizedRoleName">The role to check membership of</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a flag indicating if the specified user is a member of the given group. If the
        /// user is a member of the group the returned value with be true, otherwise it will be false.</returns>
        public abstract Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Return a role with the normalized name if it exists.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The role if it exists.</returns>
        protected abstract Task<TRole?> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken);
       
    }  

}
