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

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController<TUser> : Controller 
        where TUser : IdentityUser<Guid>, new()
    {
        private readonly UserManager<TUser> userManager;
        private readonly IEmailSender emailSender;


        public AccountController(UserManager<TUser> userManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

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
    }
}
