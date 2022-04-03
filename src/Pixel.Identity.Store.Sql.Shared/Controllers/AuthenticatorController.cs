using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Core.Controllers;
using System.Text.Encodings.Web;

namespace Pixel.Identity.Store.Sql.Shared.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticatorController : AuthenticatorController<ApplicationUser, Guid>
    {
        public AuthenticatorController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, UrlEncoder urlEncoder)
            : base(userManager, signInManager, urlEncoder)
        {
        }
    }
}
