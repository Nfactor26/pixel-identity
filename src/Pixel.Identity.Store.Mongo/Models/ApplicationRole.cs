using AspNetCore.Identity.MongoDbCore.Models;

namespace Pixel.Identity.Store.Mongo
{
    /// <summary>
    /// ApplicationRole is the <see cref="IdentityRole"/> with Guid as primary key required by Asp.Net Identity
    /// </summary>
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
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
    }
}
