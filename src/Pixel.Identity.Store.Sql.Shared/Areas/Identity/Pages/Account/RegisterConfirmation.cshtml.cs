using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Sql.Shared.Account
{
    public class RegisterConfirmationModel : RegisterConfirmationModel<ApplicationUser>
    {
        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender) : base(userManager, sender)
        {
        }
    }
}
