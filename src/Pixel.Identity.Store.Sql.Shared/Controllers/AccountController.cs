using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Controllers;

namespace Pixel.Identity.Store.Sql.Shared.Controllers
{
    public class AccountController : AccountController<ApplicationUser, Guid>
    {
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
          : base(userManager, signInManager, emailSender)
        {
        }
    }
}
