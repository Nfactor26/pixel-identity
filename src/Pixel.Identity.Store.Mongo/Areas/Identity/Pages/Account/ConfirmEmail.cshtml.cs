using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core.Pages;

namespace Pixel.Identity.Store.Mongo.Account
{
    public class ConfirmEmailModel : ConfirmEmailModel<ApplicationUser>
    {
        public ConfirmEmailModel(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}
