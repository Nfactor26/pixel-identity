using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.Provider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.IsAdmin)]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet("{roleName}")]
        public async Task<ActionResult<UserRoleViewModel>> Get(string roleName)
        {
            var role = await this.roleManager.FindByNameAsync(roleName);
            if(role != null)
            {
                return new UserRoleViewModel(role.Id, role.Name);
            }
            return NotFound();
        }

        [HttpGet()]
        public List<UserRoleViewModel> GetAll()
        {
            List<UserRoleViewModel> userRoles = new List<UserRoleViewModel>();
            var roles = this.roleManager.Roles;
            foreach(var role in roles)
            {
                userRoles.Add(new UserRoleViewModel(role.Id, role.Name));
            }
            return userRoles;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(UserRoleViewModel userRole)
        {
           var result = await  roleManager.CreateAsync(new ApplicationRole(userRole.RoleName));
           if(result.Succeeded)
           {            
                return CreatedAtAction(nameof(Get), new { name = userRole.RoleName }, userRole);
           }
            return BadRequest(new BadRequestResponse(result.Errors.Select(e => e.ToString())));
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRolesToUser([FromBody] AddUserRolesRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if(user != null)
            {
               foreach(var role in request.RolesToAdd)
                {
                    var isInRole = await userManager.IsInRoleAsync(user, role.RoleName);
                    if(!isInRole)
                    {
                        await userManager.AddToRoleAsync(user, role.RoleName);
                    }
                }
                return Ok();
            }
            return NotFound("User doesn't exist");
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRolesFromUser([FromBody] RemoveUserRolesRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                foreach (var role in request.RolesToRemove)
                {
                    var isInRole = await userManager.IsInRoleAsync(user, role.RoleName);
                    if (isInRole)
                    {
                        await userManager.RemoveFromRoleAsync(user, role.RoleName);
                    }
                }
                return Ok();
            }
            return NotFound("User doesn't exist");
        }
    }
}
