using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Mongo.Account
{
    [AllowAnonymous]
    public class RegisterModel : RegisterModel<ApplicationUser>
    {
        public RegisterModel(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager, ILogger<Core.Pages.RegisterModel> logger,
            IEmailSender emailSender) : base(userManager, userStore, signInManager, logger, emailSender)
        {
        }
    }
}
