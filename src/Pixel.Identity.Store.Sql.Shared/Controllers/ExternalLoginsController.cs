using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Pixel.Identity.Core.Controllers;

namespace Pixel.Identity.Store.Sql.Shared.Controllers
{
    public class ExternalLoginsController : ExternalLoginsController<ApplicationUser, Guid>
    {
        public ExternalLoginsController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginsController<ApplicationUser, Guid>> logger)
            : base(userManager, signInManager, userStore, logger)
        {
        }
    }
}
