using Microsoft.AspNetCore.Identity;
namespace Pixel.Identity.Store.Sql.Shared
{
    /// <summary>
    /// ApplicationUser is the <see cref="IdentityUser"/> with Guid as primary key required by Asp.Net Identity
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
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
        public ApplicationUser(string userName) : base(userName)
        {

        }
    }
}
