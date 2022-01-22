using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Pixel.Identity.Core.Controllers;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    public class AccountController : AccountController<ApplicationUser>
    {
        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender) 
            : base(userManager, emailSender)
        {
        }
    }
}
