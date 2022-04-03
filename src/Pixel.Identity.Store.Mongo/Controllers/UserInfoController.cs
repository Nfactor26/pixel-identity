using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Pixel.Identity.Core.Controllers;
using Pixel.Identity.Store.Mongo.Models;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    /// <summary>
    /// Api endpoint for retrieving user info
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : UserinfoController<ApplicationUser, ObjectId>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="userManager"></param>
        public UserInfoController(UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
        }
    }
}