using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Controllers;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    public class AccountController : AccountController<ApplicationUser>
    {
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IEmailSender emailSender) 
            : base(userManager, signInManager, emailSender)
        {
        }
    }
}
