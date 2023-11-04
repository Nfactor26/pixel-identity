using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Store.Mongo.Models;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    /// <summary>
    /// Api endpoint for managing asp.net identity users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : UsersController<ApplicationUser, ObjectId>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="userManager"></param>
        public UsersController(ILogger<UsersController<ApplicationUser, ObjectId>> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore, IEmailSender emailSender)
            : base(logger, mapper, userManager, userStore, emailSender)
        {
        }
    }
}