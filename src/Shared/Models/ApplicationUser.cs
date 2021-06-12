using System;
using AspNetCore.Identity.MongoDbCore.Models;


namespace Pixel.Identity.Shared.Models
{   
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(string userName, string email) : base(userName, email)
        {
        }
    }
}
