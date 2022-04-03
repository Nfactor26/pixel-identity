using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Store.Mongo.Models;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    public class ExternalLoginsController : ExternalLoginsController<ApplicationUser, ObjectId>
    {
        public ExternalLoginsController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginsController<ApplicationUser, ObjectId>> logger) 
            : base(userManager, signInManager, userStore, logger)
        {
        }
    }
}
