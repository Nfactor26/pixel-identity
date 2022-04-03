using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Store.Mongo.Models;
using System.Text.Encodings.Web;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticatorController : AuthenticatorController<ApplicationUser, ObjectId>
    {
        public AuthenticatorController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, UrlEncoder urlEncoder)
            : base(userManager, signInManager, urlEncoder)
        {
        }
    }
}
