using Microsoft.AspNetCore.Identity;

namespace Pixel.Identity.Store.Mongo.Models
{
    public class MongoDbSettings
    {
        /// <summary>
        /// Connection string for the the MongoDb
        /// </summary>
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        /// <summary>
        /// Name of the Database
        /// </summary>
        public string DatabaseName { get; set; } = "pixel-identity-db";

        /// <summary>
        /// Collection used to store <see cref="IdentityUser{TKey}"/>
        /// </summary>
        public string UsersCollection { get; set; } = "applicationUsers";

        /// <summary>
        /// Collection used to store <see cref="IdentityRole{TKey}"/>
        /// </summary>
        public string RolesCollection { get; set; } = "applicationRoles";
    }
}
