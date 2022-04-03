using Dawn;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Pixel.Identity.Store.Mongo.Models;
using Pixel.Identity.Store.Mongo.Stores;
using Pixel.Identity.Store.Mongo.Utils;
using System.ComponentModel;

namespace Pixel.Identity.Store.Mongo.Extensions
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddMongoDbStore<TUser, TRole, TUserClaim, TUserLogin, TUserToken, TRoleClaim, TKey>(this IdentityBuilder builder, MongoDbSettings dbSettings)
              where TUser : ApplicationUser<TKey, TUserClaim, TUserLogin, TUserToken>, new ()
              where TRole : ApplicationRole<TKey>, new ()
              where TUserClaim : IdentityUserClaim<TKey>, new()
              where TRoleClaim : IdentityRoleClaim<TKey>, new()
              where TUserLogin : IdentityUserLogin<TKey>, new()
              where TUserToken : IdentityUserToken<TKey>, new()
              where TKey : IEquatable<TKey>
        {
            Guard.Argument(dbSettings).NotNull();
            Guard.Argument(dbSettings.DatabaseName).NotNull().NotEmpty();
            Guard.Argument(dbSettings.ConnectionString).NotNull().NotEmpty();

            var mongoClient = new MongoClient(dbSettings.ConnectionString);
            var mongoDb = mongoClient.GetDatabase(dbSettings.DatabaseName);
            var usersCollection = mongoDb.GetCollection<TUser>(dbSettings.UsersCollection);
            var rolesCollection = mongoDb.GetCollection<TRole>(dbSettings.RolesCollection);
            var identityErrorDescriber = new IdentityErrorDescriber();

            builder.Services.TryAddScoped<IUserStore<TUser>>(provider =>
            {
                return new UserStore<TUser, TRole, TUserClaim, TUserLogin, TUserToken, TRoleClaim, TKey>(usersCollection, rolesCollection, identityErrorDescriber);
            });

            builder.Services.TryAddScoped<IRoleStore<TRole>>(provider =>
            {
                return new RoleStore<TRole, TRoleClaim, TKey>(rolesCollection, identityErrorDescriber);
            });

            return builder;
        }
    }
}
