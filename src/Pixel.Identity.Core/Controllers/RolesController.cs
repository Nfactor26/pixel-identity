using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System.Security.Claims;

namespace Pixel.Identity.Core.Controllers
{
    /// <summary>
    /// Api endpoint for managing asp.net identity roles.
    /// It must be inherited in DbStore plugin which should provide the desired TUser and TRole
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageRoles)]
    public class RolesController<TUser, TRole, TKey> : ControllerBase
        where TUser : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> roleManager;
        private readonly UserManager<TUser> userManager;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="roleManager">Asp.Net Identity <see cref="RoleManager{TRole}"/></param>
        /// <param name="userManager">Asp.Net Identity <see cref="UserManager{TUser}"/></param>
        public RolesController(RoleManager<TRole> roleManager, UserManager<TUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// Get the details of role given role name
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        /// <returns></returns>
        [HttpGet("{roleName}")]
        public async Task<ActionResult<UserRoleViewModel>> Get(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await this.roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var userRoleViewModel = new UserRoleViewModel(role.Id.ToString(), role.Name);
                    var claims = await this.roleManager.GetClaimsAsync(role) ?? Enumerable.Empty<Claim>();
                    foreach (var claim in claims)
                    {
                        userRoleViewModel.Claims.Add(ClaimViewModel.FromClaim(claim));
                    }
                    return userRoleViewModel;
                }
            }
            return NotFound(new NotFoundResponse($"{roleName ?? ""} could not be located"));
        }

        /// <summary>
        /// Get the available roles as a paging list i.e. n items at a time.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public PagedList<UserRoleViewModel> GetAll([FromQuery] GetRolesRequest getRolesRequest)
        {
            List<UserRoleViewModel> userRoles = new();
            int count = 0;
            IQueryable<TRole> roles = default;
            if (!string.IsNullOrEmpty(getRolesRequest.RoleFilter))
            {
                count = this.roleManager.Roles.Where(r => r.Name.Contains(getRolesRequest.RoleFilter)).Count();
                roles = this.roleManager.Roles.Where(r => r.Name.Contains(getRolesRequest.RoleFilter))
                    .Skip(getRolesRequest.Skip).Take(getRolesRequest.Take).OrderBy(r => r.Name);
            }
            else
            {
                count = this.roleManager.Roles.Count();
                roles = this.roleManager.Roles.Skip(getRolesRequest.Skip).Take(getRolesRequest.Take).OrderBy(r => r.Name);
            }
            foreach (var role in roles)
            {
                userRoles.Add(new UserRoleViewModel(role.Id.ToString(), role.Name));
            }
            return new PagedList<UserRoleViewModel>(userRoles, count, getRolesRequest.CurrentPage, getRolesRequest.PageSize);
        }

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="userRole">name of the role to add</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddRoleAsync(UserRoleViewModel userRole)
        {
            if (ModelState.IsValid)
            {
                var result = await roleManager.CreateAsync(new TRole() { Name = userRole.RoleName });
                if (result.Succeeded)
                {
                    var role = await roleManager.FindByNameAsync(userRole.RoleName);
                    foreach (var userClaim in userRole.Claims ?? Enumerable.Empty<ClaimViewModel>())
                    {
                        await roleManager.AddClaimAsync(role, userClaim.ToClaim());
                    }
                    return Ok();
                }
                return BadRequest(new BadRequestResponse(result.Errors.Select(e => e.ToString())));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Update role name to a new value
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateRoleNameAsync(UpdateRoleNameRequest request)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(request.RoleId);
                if (role == null)
                {
                    return NotFound(new NotFoundResponse($"Failed to find role with Id : {request.RoleId}"));
                }
                if ((!role.Name?.Equals(request.NewName)) ?? false)
                {
                    await roleManager.SetRoleNameAsync(role, request.NewName);
                    await roleManager.UpdateAsync(role);                    
                }
                return Ok();
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }


        [HttpDelete("{roleName}")]
        public async Task<IActionResult> Delete(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var users = await userManager.GetUsersInRoleAsync(roleName);
                    if (!users.Any())
                    {
                        var result = await roleManager.DeleteAsync(role);
                        if (result.Succeeded)
                        {
                            return Ok();
                        }
                        return BadRequest(new BadRequestResponse(result.Errors.Select(e => e.ToString())));
                    }
                    return BadRequest(new BadRequestResponse(new[] { $"There are users with assigned role : {roleName}." }));
                }
                return NotFound(new NotFoundResponse($"Role : {roleName} doesn't exist."));
            }
            return BadRequest(new BadRequestResponse(new[] { "No role specified to delete" }));
        }

        /// <summary>
        /// Add a new claim to a role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add/claim")]
        public async Task<IActionResult> AddClaimToRole([FromBody] AddClaimRequest request)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByNameAsync(request.Owner);
                if (role != null)
                {
                    var claims = await this.roleManager.GetClaimsAsync(role);
                    foreach (var claim in claims)
                    {
                        if (claims.Any(a => a.Type.Equals(request.ClaimToAdd.Type) && a.Value.Equals(request.ClaimToAdd.Value)))
                        {
                            return BadRequest(new BadRequestResponse(new[] { "Claim already exists for role" }));
                        }
                    }
                    await roleManager.AddClaimAsync(role, request.ClaimToAdd.ToClaim());
                    return Ok();
                }
                return NotFound(new NotFoundResponse($"Role : {request.Owner} not found."));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Modify details of an existing claim on role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("update/claim")]
        public async Task<IActionResult> UpdateClaimForRole([FromBody] UpdateClaimRequest request)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByNameAsync(request.Owner);
                if (role != null)
                {
                    var claims = await this.roleManager.GetClaimsAsync(role);
                    var claimToRemove = claims.FirstOrDefault(c => c.Type.Equals(request.Original.Type)
                        && c.Value.Equals(request.Original.Value));
                    if (claimToRemove != null)
                    {
                        await roleManager.RemoveClaimAsync(role, claimToRemove);
                        await roleManager.AddClaimAsync(role, request.Modified.ToClaim());
                    }
                    return Ok();
                }
                return NotFound(new NotFoundResponse($"Role : {request.Owner} not found."));

            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Delete an existing claim from a role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("delete/claim")]
        public async Task<IActionResult> RemoveClaimFromRole([FromBody] RemoveClaimRequest request)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByNameAsync(request.Owner);
                if (role != null)
                {
                    var claims = await this.roleManager.GetClaimsAsync(role);
                    if (claims != null)
                    {
                        var claimToRemove = claims.FirstOrDefault(a => a.Type.Equals(request.ClaimToRemove.Type) && a.Value.Equals(request.ClaimToRemove.Value));
                        if(claimToRemove != null)
                        {
                            await roleManager.RemoveClaimAsync(role, claimToRemove);
                            return Ok();
                        }
                    }                   
                    return NotFound(new NotFoundResponse($"Claim doesn't exist on role."));
                }
                return NotFound(new NotFoundResponse($"Role : {request.Owner} not found."));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Assign specified role to a given user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRolesToUser([FromBody] AddUserRolesRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                foreach (var role in request.RolesToAdd)
                {
                    var isInRole = await userManager.IsInRoleAsync(user, role.RoleName);
                    if (!isInRole)
                    {
                        await userManager.AddToRoleAsync(user, role.RoleName);
                    }
                }
                return Ok();
            }
            return NotFound(new NotFoundResponse("User doesn't exist"));
        }       

        /// <summary>
        /// Remove specified role from a given user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
            return NotFound(new NotFoundResponse("User doesn't exist"));
        }
    }
}
