using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Sql.Shared.Account
{
    public class ConfirmEmailModel : ConfirmEmailModel<ApplicationUser>
    {
        public ConfirmEmailModel(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}
