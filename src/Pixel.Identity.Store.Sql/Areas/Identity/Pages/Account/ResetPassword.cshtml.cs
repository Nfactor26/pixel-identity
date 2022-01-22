using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Sql.Account
{
    public class ResetPasswordModel : ResetPasswordModel<ApplicationUser>
    {
        public ResetPasswordModel(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}
