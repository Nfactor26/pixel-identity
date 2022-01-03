using Microsoft.AspNetCore.Identity;

namespace Pixel.Identity.Store.Sql
{
    /// <summary>
    /// ApplicationRole is the <see cref="IdentityRole"/> with Guid as primary key required by Asp.Net Identity
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ApplicationRole()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
