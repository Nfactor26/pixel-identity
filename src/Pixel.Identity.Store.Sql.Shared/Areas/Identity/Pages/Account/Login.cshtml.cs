using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Sql.Shared.Account
{
    public class LoginModel : LoginModel<ApplicationUser>
    {
        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
            : base(signInManager, logger)
        {

        }
    }
}
