using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared;
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

        public UsersController(IMapper mapper, UserManager<TUser> userManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;           
        }          
       
        [HttpGet]
        public async Task<IEnumerable<UserDetailsViewModel>> GetAll()
        {
            var applicationUsers = this.userManager.Users ?? Enumerable.Empty<TUser>();
            var userDetails = mapper.Map<IEnumerable<UserDetailsViewModel>>(applicationUsers);
            return await Task.FromResult(userDetails);
        }

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
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserDetailsViewModel userDetails)
        {
           if(ModelState.IsValid)
           {
                var user = await userManager.FindByEmailAsync(userDetails.Email);              
                if(!user.UserName.Equals(userDetails.UserName))
                {
                    await userManager.SetUserNameAsync(user, userDetails.UserName);                  
                }
                if (!user.Email.Equals(userDetails.Email))
                {
                    await userManager.SetEmailAsync(user, userDetails.Email);
                }
                if(!user.PhoneNumber.Equals(userDetails.PhoneNumber))
                {
                    await userManager.SetPhoneNumberAsync(user, userDetails.PhoneNumber);                   
                }
                return Ok();
            }
            return BadRequest(ModelState);

        }       
    }
}
