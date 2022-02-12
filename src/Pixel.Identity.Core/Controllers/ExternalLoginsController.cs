using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared.Responses;

namespace Pixel.Identity.Core.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalLoginsController<TUser> : Controller
        where TUser : IdentityUser<Guid>, new()
    {
        private readonly UserManager<TUser> userManager;
        private readonly SignInManager<TUser> signInManager;
        private readonly IUserStore<TUser> userStore;
        private readonly ILogger<ExternalLoginsController<TUser>> logger;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>       
        public ExternalLoginsController(UserManager<TUser> userManager,
            SignInManager<TUser> signInManager, IUserStore<TUser> userStore,
            ILogger<ExternalLoginsController<TUser>> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userStore = userStore;
            this.logger = logger;
        }

        /// <summary>
        /// Get the external logins for user account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetExternalLogins()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse("User could not be loaded."));
            }
            var currentLogins = await userManager.GetLoginsAsync(user);
            return Ok(currentLogins);
        }
       
        /// <summary>
        /// Remove external login from user account
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        [HttpDelete("{loginProvider}/{providerKey}")]
        public async Task<IActionResult> RemoveExternalLogin(string loginProvider, string providerKey)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse("User could not be loaded."));
            }

            var currentLogins = await userManager.GetLoginsAsync(user);
            string passwordHash = string.Empty;
            if (userStore is IUserPasswordStore<TUser> userPasswordStore)
            {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
            }

            string hasPassword = string.IsNullOrEmpty(passwordHash) ? "no password" : "local password";
            logger.LogInformation($"User has {currentLogins.Count()} external logins and {hasPassword}");

            if(!string.IsNullOrEmpty(passwordHash) || currentLogins.Count > 1)
            {
                var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
                if (!result.Succeeded)
                {
                    logger.LogError($"Error while removing external login provider {loginProvider} for user {await userManager.GetUserNameAsync(user)}." +
                        $"{String.Join(';', result.Errors)}");
                    return Problem($"Error while removing external login provider : {loginProvider}");
                }
                logger.LogInformation($"External login {loginProvider} removed successfully for user {await userManager.GetUserNameAsync(user)}");
                await signInManager.RefreshSignInAsync(user);
                return Ok();
            }

            return Problem($"Error while removing external login provider : {loginProvider}");
        }
    }
}
