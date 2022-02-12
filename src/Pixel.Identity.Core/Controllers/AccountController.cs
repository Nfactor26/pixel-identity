using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.Responses;
using System.Text;
using System.Text.Encodings.Web;

namespace Pixel.Identity.Core.Controllers
{
    /// <summary>
    /// Api endpoint for managing user account
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController<TUser> : Controller 
        where TUser : IdentityUser<Guid>, new()
    {
        private readonly UserManager<TUser> userManager;
        private readonly SignInManager<TUser> signInManager;
        private readonly IEmailSender emailSender;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="emailSender"></param>
        public AccountController(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }

        /// <summary>
        /// Send reset password link to user registered email address which can be used to 
        /// reset user password by the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("password/sendresetlink")]
        public async Task<IActionResult> SendResetPasswordLink([FromBody] ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Ok();
                }

                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/Account/ResetPassword", pageHandler: null, 
                    values: new { area = "Identity", code }, protocol: Request.Scheme);
                await emailSender.SendEmailAsync(model.Email, "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return Ok();
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Update user password to a new value
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new NotFoundResponse("User could not be loaded."));
                }

                var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    return BadRequest(new BadRequestResponse(changePasswordResult.Errors.Select(e => e.ToString())));
                }
                await signInManager.RefreshSignInAsync(user);               
                return Ok();
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));            
        }

        /// <summary>
        /// Check if user has a password
        /// </summary>
        /// <returns>true if user has password, false otherwise</returns>
        [Authorize]
        [HttpGet("haspassword")]
        public async Task<bool> HasPassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return false;
            }
            return await userManager.HasPasswordAsync(user);
        }

        /// <summary>
        /// Update user password to a new value
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("password/set")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new NotFoundResponse("User could not be loaded."));
                }

                var hasPassword = await userManager.HasPasswordAsync(user);
                if (hasPassword)
                {
                    return BadRequest(new BadRequestResponse(new [] { "Password is already set."}));
                }

                var addPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    return BadRequest(new BadRequestResponse(addPasswordResult.Errors.Select(e => e.ToString())));
                }

                await signInManager.RefreshSignInAsync(user);
                return Ok();
            }

            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));           
        }

        /// <summary>
        /// Send a verification link to user registered email. User can click this link to verify
        /// the new email. Once verified email and name are automatically updated to new email value.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("email/change")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new NotFoundResponse("User could not be loaded."));
                }

                var email = await userManager.GetEmailAsync(user);
                if (model.NewEmail != email)
                {
                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page("/Account/ConfirmEmailChange", pageHandler: null,
                        values: new { area = "Identity", userId = userId, email = model.NewEmail, code = code },
                        protocol: Request.Scheme);
                    await emailSender.SendEmailAsync(model.NewEmail, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    return Ok();    
                }

                return BadRequest(new BadRequestResponse(new[] { "Email could not be changed." }));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Resend email verification link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("email/resendconfirm")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Ok(); // Don't reveal that email doesn't exist
                }

                var userId = await userManager.GetUserIdAsync(user);
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code }, protocol: Request.Scheme);
                await emailSender.SendEmailAsync(model.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return Ok();
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>      
        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new NotFoundResponse("User could not be loaded."));
                }
                if (!await userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");                    
                }
                else
                {
                    var result = await userManager.DeleteAsync(user);
                    var userId = await userManager.GetUserIdAsync(user);
                    if (!result.Succeeded)
                    {
                        return Problem("Unexpected error occurred deleting user.");
                    }

                    await signInManager.SignOutAsync();
                    return Ok();
                }
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }
    }
}
