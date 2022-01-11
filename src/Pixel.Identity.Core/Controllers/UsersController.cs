using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.Core.Controllers
{
    /// <summary>
    /// Api endpoint for managing asp.net identity users
    /// It must be inherited in DbStore plugin which should provide the desired TUser
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageUsers)]
    public class UsersController<TUser> : Controller
        where TUser : IdentityUser<Guid>, new()
    {
        private readonly IMapper mapper;
        private readonly UserManager<TUser> userManager;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mapper">Implementation of <see cref="IMapper"/> for mapping models</param>
        /// <param name="userManager">Asp.Net Identity <see cref="UserManager{TUser}"/></param>
        public UsersController(IMapper mapper, UserManager<TUser> userManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;           
        }

        /// <summary>
        /// Get the available users as a paging list i.e. n items at a time.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public PagedList<UserDetailsViewModel> GetAll([FromQuery] GetUsersRequest request)
        {
            int count = 0;
            IEnumerable<TUser> applicationUsers;
            if(!string.IsNullOrEmpty(request.UsersFilter))
            {
                count = this.userManager.Users.Where(u => u.UserName.StartsWith(request.UsersFilter)
                || u.Email.StartsWith(request.UsersFilter)).Count();
                applicationUsers = this.userManager.Users.Where(u => u.UserName.StartsWith(request.UsersFilter)
                || u.Email.StartsWith(request.UsersFilter)).Skip(request.Skip).Take(request.Take).OrderBy(u => u.UserName);
            }           
            else
            {
                count = this.userManager.Users.Count();
                applicationUsers = this.userManager.Users.OrderBy(u => u.UserName);
            }
           
            var userDetails = mapper.Map<IEnumerable<UserDetailsViewModel>>(applicationUsers);
            return new PagedList<UserDetailsViewModel>() 
            { 
                Items = new List<UserDetailsViewModel>(userDetails),
                ItemsCount = count, 
                CurrentPage = request.CurrentPage,
                PageCount = request.PageSize
            };
        }

        /// <summary>
        /// Get the details of a user given user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("{userName}")]
        public async Task<ActionResult<UserDetailsViewModel>> GetUserByName(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if(user != null)
            {
                var userDetails = mapper.Map<UserDetailsViewModel>(user);
                var userRoles = await this.userManager.GetRolesAsync(user);
                foreach (var userRole in userRoles)
                {
                    userDetails.UserRoles.Add(new UserRoleViewModel(userRole));
                }               
                return userDetails;
            }
            return NotFound(new NotFoundResponse($"User with name : {userName} doesn't exist."));
        }

        /// <summary>
        /// Update users details like
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(UserDetailsViewModel userDetails)
        {
           if(ModelState.IsValid)
           {
                var user = await userManager.FindByEmailAsync(userDetails.Email);  
                if(user != null)
                {
                    await userManager.SetTwoFactorEnabledAsync(user, userDetails.TwoFactorEnabled);
                    await userManager.SetLockoutEnabledAsync(user, userDetails.LockoutEnabled);
                    if (user.LockoutEnd.HasValue)
                    {
                        await userManager.SetLockoutEndDateAsync(user, userDetails.LockoutEnd);
                    }
                    return Ok();
                }
                return NotFound(new NotFoundResponse("User doesn't exist."));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));

        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> Delete(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    await userManager.DeleteAsync(user);
                    return Ok();
                }
                return NotFound(new NotFoundResponse("User doesn't exist."));
            }
            return BadRequest(new BadRequestResponse(new[] { "UserName is required" }));
        }
    }
}
