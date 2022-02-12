using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Sql.Account
{
    public class ExternalLoginModel : ExternalLoginModel<ApplicationUser>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        /// <param name="userStore"></param>
        /// <param name="logger"></param>
        /// <param name="emailSender"></param>
        public ExternalLoginModel(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, 
            ILogger<Core.Pages.ExternalLoginModel> logger, IEmailSender emailSender) 
            : base(signInManager, userManager, userStore, logger, emailSender)
        {

        }
    }
}
