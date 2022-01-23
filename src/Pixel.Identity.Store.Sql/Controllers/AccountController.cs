using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Pixel.Identity.Core.Controllers;

namespace Pixel.Identity.Store.Sql.Controllers
{
    public class AccountController : AccountController<ApplicationUser>
    {
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
          : base(userManager, signInManager, emailSender)
        {
        }
    }
}
