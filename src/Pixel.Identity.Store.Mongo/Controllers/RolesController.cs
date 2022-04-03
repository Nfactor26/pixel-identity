using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Shared;
using Pixel.Identity.Store.Mongo.Models;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    /// <summary>
    /// Api endpoint for managing asp.net identity roles
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageRoles)]
    public class RolesController : RolesController<ApplicationUser, ApplicationRole, ObjectId>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        public RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager) :
            base(roleManager, userManager)
        {
        }
    }
}
