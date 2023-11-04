using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Core;
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
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="userManager"></param>
        /// <param name="userStore"></param>
        /// <param name="emailSender"></param>
        public UsersController(ILogger<UsersController<ApplicationUser, Guid>> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore, IEmailSender emailSender)
            : base(logger, mapper, userManager, userStore, emailSender)
        {
        }
    }
}