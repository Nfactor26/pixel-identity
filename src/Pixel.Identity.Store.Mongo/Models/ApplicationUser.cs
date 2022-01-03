using AspNetCore.Identity.MongoDbCore.Models;

namespace Pixel.Identity.Store.Mongo
{
    /// <summary>
    /// ApplicationUser is the <see cref="IdentityUser"/> with Guid as primary key required by Asp.Net Identity
    /// </summary>
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
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
        public ApplicationUser(string userName, string email) : base(userName, email)
        {
        }
    }
}
