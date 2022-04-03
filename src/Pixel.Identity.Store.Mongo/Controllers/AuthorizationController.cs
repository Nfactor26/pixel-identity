using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using OpenIddict.Abstractions;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Store.Mongo.Models;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    /// <summary>
    /// Controller for handling OpenId protocol using OpenIdDict.
    /// It provides end points for authentication, tokens, sign out , etc.
    /// </summary>
    public class AuthorizationController : AuthorizationController<ApplicationUser, ObjectId>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="applicationManager"></param>
        /// <param name="authorizationManager"></param>
        /// <param name="scopeManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        public AuthorizationController(IOpenIddictApplicationManager applicationManager, IOpenIddictAuthorizationManager authorizationManager, IOpenIddictScopeManager scopeManager, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) :
            base(applicationManager, authorizationManager, scopeManager, signInManager, userManager)
        {
        }
    }
}
