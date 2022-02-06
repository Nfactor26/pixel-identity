using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Sql.Account
{
    public class LoginWith2faModel : LoginWith2faModel<ApplicationUser>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        public LoginWith2faModel(SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager) : base(signInManager, userManager)
        {
        }
    }
}
