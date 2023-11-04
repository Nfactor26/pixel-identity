using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System.Text;
using System.Text.Encodings.Web;

namespace Pixel.Identity.Core.Controllers
{
    /// <summary>
    /// Api endpoint for managing asp.net identity users
    /// It must be inherited in DbStore plugin which should provide the desired TUser
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]    
    public class UsersController<TUser, TKey> : Controller
        where TUser : IdentityUser<TKey>, new()
        where TKey :  IEquatable<TKey>
    {
        private readonly IMapper mapper;
        private readonly UserManager<TUser> userManager;
        private readonly IUserStore<TUser> userStore;
        private readonly IUserEmailStore<TUser> emailStore;
        private readonly ILogger<UsersController<TUser, TKey>> logger;
        private readonly IEmailSender emailSender;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mapper">Implementation of <see cref="IMapper"/> for mapping models</param>
        /// <param name="userManager">Asp.Net Identity <see cref="UserManager{TUser}"/></param>
        public UsersController(ILogger<UsersController<TUser, TKey>> logger, IMapper mapper, UserManager<TUser> userManager, 
            IUserStore<TUser> userStore, IEmailSender emailSender)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
            this.userStore = userStore;
            this.emailStore = GetEmailStore();
            this.emailSender = emailSender;
        }

        IUserEmailStore<TUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<TUser>)userStore;
        }

        /// <summary>
        /// Get the available users as a paging list i.e. n items at a time.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = Policies.CanManageUsers)]
        public PagedList<UserDetailsViewModel> GetAll([FromQuery] GetUsersRequest request)
        {
            int count = 0;
            IEnumerable<TUser> applicationUsers;
            if (!string.IsNullOrEmpty(request.UsersFilter))
            {
                count = this.userManager.Users.Where(u => u.UserName.Contains(request.UsersFilter)
                || u.Email.Contains(request.UsersFilter)).Skip(request.Skip).Take(request.Take).Count();
                applicationUsers = this.userManager.Users.Where(u => u.UserName.Contains(request.UsersFilter)
                || u.Email.Contains(request.UsersFilter)).Skip(request.Skip).Take(request.Take).OrderBy(u => u.UserName);
            }
            else
            {
                count = this.userManager.Users.Count();
                applicationUsers = this.userManager.Users.Skip(request.Skip).Take(request.Take).OrderBy(u => u.UserName);
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
        [HttpGet("name/{userName}")]
        public async Task<ActionResult<UserDetailsViewModel>> GetUserByName(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var userDetails = mapper.Map<UserDetailsViewModel>(user);
                var userRoles = await this.userManager.GetRolesAsync(user);
                foreach (var userRole in userRoles)
                {
                    userDetails.UserRoles.Add(new UserRoleViewModel(userRole));
                }
                var userClaims = await this.userManager.GetClaimsAsync(user);
                foreach (var claim in userClaims)
                {
                    userDetails.UserClaims.Add(ClaimViewModel.FromClaim(claim));
                }
                return userDetails;
            }
            return NotFound(new NotFoundResponse($"User with name : {userName} doesn't exist."));
        }


        [HttpGet("id/{userId}")]
        public async Task<ActionResult<UserDetailsViewModel>> GetUserById(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userDetails = mapper.Map<UserDetailsViewModel>(user);
                var userRoles = await this.userManager.GetRolesAsync(user);
                foreach (var userRole in userRoles)
                {
                    userDetails.UserRoles.Add(new UserRoleViewModel(userRole));
                }
                var userClaims = await this.userManager.GetClaimsAsync(user);
                foreach(var claim in userClaims)
                {
                    userDetails.UserClaims.Add(ClaimViewModel.FromClaim(claim));
                }
                return userDetails;
            }
            return NotFound(new NotFoundResponse($"User with Id : {userId} doesn't exist"));
        }

        [HttpPost("lock")]
        [Authorize(Policy = Policies.CanManageUsers)]
        public async Task<IActionResult> LockUserAsync([FromBody] string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.SetLockoutEnabledAsync(user, true);
                await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(90));
                await userManager.UpdateSecurityStampAsync(user);
                return Ok();                
            }
            return NotFound(new NotFoundResponse($"User with Id : {userId} doesn't exist"));
        }

        [HttpPost("unlock")]
        [Authorize(Policy = Policies.CanManageUsers)]
        public async Task<IActionResult> UnlockUserAsync([FromBody] string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await userManager.SetLockoutEndDateAsync(user, null);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(new BadRequestResponse(result.GetErrors()));
            }
            return NotFound(new NotFoundResponse($"User with Id : {userId} doesn't exist"));
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Authorize(Policy = Policies.CanManageUsers)]
        public async Task<IActionResult> AddUserAsync(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = CreateUser();

                await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");

                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = Url.Content("~/") },
                        protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    return Ok();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
                }

            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));

            TUser CreateUser()
            {
                try
                {
                    return Activator.CreateInstance<TUser>();
                }
                catch
                {
                    throw new InvalidOperationException($"Can't create an instance of '{nameof(TUser)}'. " +
                        $"Ensure that '{nameof(TUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                        $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
                }
            }
        }

        /// <summary>
        /// Update users details like
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        [HttpPost("{userId}")]
        [Authorize(Policy = Policies.CanManageUsers)]
        public async Task<IActionResult> Post(string userId, UserDetailsViewModel userDetails)
        {
           if(!string.IsNullOrEmpty(userId) && ModelState.IsValid)
           {
                var user = await userManager.FindByIdAsync(userId);  
                if(user != null)
                {
                    user.UserName = userDetails.Email; // done on purpose. we use email for both username and email
                    user.Email = userDetails.Email;
                    user.EmailConfirmed = true;
                    user.LockoutEnabled = userDetails.LockoutEnabled;
                    if (!string.IsNullOrEmpty(userDetails.PhoneNumber))
                    {
                        user.PhoneNumber = userDetails.PhoneNumber;
                    }
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    return BadRequest(new BadRequestResponse(result.GetErrors()));
                }
                return NotFound(new NotFoundResponse("User doesn't exist."));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));

        }

        [HttpDelete("{userName}")]
        [Authorize(Policy = Policies.CanManageUsers)]
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

        [HttpPost("add/claim")]
        [Authorize(Policy = Policies.CanManageUsers)]
        public async Task<IActionResult> AddUserClaim([FromBody] AddClaimRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(request.Owner);
                if (user != null)
                {
                    var claims = await this.userManager.GetClaimsAsync(user);
                    foreach (var claim in claims)
                    {
                        if (claims.Any(a => a.Type.Equals(request.ClaimToAdd.Type) && a.Value.Equals(request.ClaimToAdd.Value)))
                        {
                            return BadRequest(new BadRequestResponse(new[] { "Claim already exists for role" }));
                        }
                    }
                    await userManager.AddClaimAsync(user, request.ClaimToAdd.ToClaim());
                    return Ok();
                }
                return NotFound(new NotFoundResponse($"User : {request.Owner} not found."));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        [HttpPost("update/claim")]
        [Authorize(Policy = Policies.CanManageUsers)]
        public async Task<IActionResult> UpdateUserClaim([FromBody] UpdateClaimRequest request)
        {

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(request.Owner);
                if (user != null)
                {
                    var claims = await this.userManager.GetClaimsAsync(user);
                    var claimToRemove = claims.FirstOrDefault(c => c.Type.Equals(request.Original.Type)
                        && c.Value.Equals(request.Original.Value));
                    if (claimToRemove != null)
                    {
                        await userManager.RemoveClaimAsync(user, claimToRemove);
                        await userManager.AddClaimAsync(user, request.Modified.ToClaim());
                    }
                    return Ok();
                }
                return NotFound(new NotFoundResponse($"User : {request.Owner} not found."));

            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        [HttpPost("delete/claim")]
        [Authorize(Policy = Policies.CanManageUsers)]
        public async Task<IActionResult> DeleteUserClaim([FromBody] RemoveClaimRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(request.Owner);
                if (user != null)
                {
                    var claims = await this.userManager.GetClaimsAsync(user);
                    if (claims != null)
                    {
                        var claimToRemove = claims.FirstOrDefault(a => a.Type.Equals(request.ClaimToRemove.Type) && a.Value.Equals(request.ClaimToRemove.Value));
                        if (claimToRemove != null)
                        {
                            await userManager.RemoveClaimAsync(user, claimToRemove);
                            return Ok();
                        }
                    }
                    return NotFound(new NotFoundResponse($"Claim doesn't exist on role."));
                }
                return NotFound(new NotFoundResponse($"Role : {request.Owner} not found."));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }


    }
}
