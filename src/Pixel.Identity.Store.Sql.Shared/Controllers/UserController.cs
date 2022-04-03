using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Core.Controllers;

namespace Pixel.Identity.Store.Sql.Shared.Controllers
{
    /// <summary>
    /// Api endpoint for managing asp.net identity users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : UsersController<ApplicationUser, Guid>
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