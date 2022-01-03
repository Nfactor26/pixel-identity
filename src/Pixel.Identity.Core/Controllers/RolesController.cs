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
    /// Api endpoint for managing asp.net identity roles.
    /// It must be inherited in DbStore plugin which should provide the desired TUser and TRole
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageRoles)]
    public class RolesController<TUser, TRole> : ControllerBase
        where TUser : IdentityUser<Guid>, new()
        where TRole : IdentityRole<Guid>, new()
    {
        private readonly RoleManager<TRole> roleManager;
        private readonly UserManager<TUser> userManager;

        public RolesController(RoleManager<TRole> roleManager, UserManager<TUser> userManager)
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
                var userRoleViewModel = new UserRoleViewModel(role.Id, role.Name);
                var claims = await this.roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)                
                {
                    userRoleViewModel.Claims.Add(new ClaimViewModel(claim.Type, claim.Value));
                }
                return userRoleViewModel;
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
        public async Task<IActionResult> AddRoleAsync(UserRoleViewModel userRole)
        {
           var result = await  roleManager.CreateAsync(new TRole() { Name = userRole.RoleName });
           if(result.Succeeded)
           {            
                return CreatedAtAction(nameof(Get), new { name = userRole.RoleName }, userRole);
           }
            return BadRequest(new BadRequestResponse(result.Errors.Select(e => e.ToString())));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleAsync(UserRoleViewModel userRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = await roleManager.FindByNameAsync(userRole.RoleName);                   
                    if (role != null)
                    {
                        var claims = await this.roleManager.GetClaimsAsync(role);
                        
                        //Track new claims and add to role
                        foreach(var claim in userRole.Claims)
                        {
                            if(claims.Any(a => a.Type.Equals(claim.Type) && a.Value.Equals(claim.Value)))
                            {
                                continue;
                            }
                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, claim.Value));   
                        }
                    
                        //Track removed claims and delete from role
                        foreach(var claim in claims)
                        {
                            if(userRole.Claims.Any(c => c.Type.Equals(claim.Type) && c.Value.Equals(claim.Value)))
                            {
                                continue;
                            }
                            await roleManager.RemoveClaimAsync(role, claim);
                        }
                        
                        return Ok();
                    }                   
                    return NotFound(new NotFoundResponse($"Failed to find role with name : {userRole.RoleName}"));
                }
                return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemResponse(ex.Message));
            }
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
