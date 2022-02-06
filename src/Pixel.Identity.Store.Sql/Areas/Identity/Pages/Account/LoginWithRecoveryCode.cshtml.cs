using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Sql.Account
{
    public class LoginWithRecoveryCodeModel : LoginWithRecoveryCodeModel<ApplicationUser>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        public LoginWithRecoveryCodeModel(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager) : base(signInManager, userManager)
        {
        }
    }
}
