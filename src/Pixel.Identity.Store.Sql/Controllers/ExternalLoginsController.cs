using Microsoft.AspNetCore.Identity;
using Pixel.Identity.Core.Controllers;

namespace Pixel.Identity.Store.Sql.Controllers
{
    public class ExternalLoginsController : ExternalLoginsController<ApplicationUser>
    {
        public ExternalLoginsController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginsController<ApplicationUser>> logger) 
            : base(userManager, signInManager, userStore, logger)
        {
        }
    }
}
