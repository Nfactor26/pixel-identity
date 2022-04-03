using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Store.Mongo.Models;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    public class AccountController : AccountController<ApplicationUser, ObjectId>
    {
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IEmailSender emailSender) 
            : base(userManager, signInManager, emailSender)
        {
        }
    }
}
