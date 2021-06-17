using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Pixel.Identity.Provider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public UsersController(IMapper mapper, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }          
       
        [HttpGet]
        public async Task<IEnumerable<UserDetailsViewModel>> GetAll()
        {
            var applicationUsers = this.userManager.Users ?? Enumerable.Empty<ApplicationUser>();
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
                var userRoles = roleManager.Roles.Where(r => user.Roles.Contains(r.Id));
                if(userRoles.Any())
                {
                    userDetails.UserRoles.AddRange(mapper.Map<IEnumerable<UserRoleViewModel>>(userRoles));
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
