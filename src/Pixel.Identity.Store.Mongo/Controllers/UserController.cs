using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Shared;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    /// <summary>
    /// Api endpoint for managing asp.net identity users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageUsers)]
    public class UsersController : UsersController<ApplicationUser>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="userManager"></param>
        public UsersController(IMapper mapper, UserManager<ApplicationUser> userManager)
            : base(mapper, userManager)
        {
        }
    }
}