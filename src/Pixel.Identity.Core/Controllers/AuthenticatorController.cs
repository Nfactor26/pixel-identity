using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.Responses;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace Pixel.Identity.Core.Controllers
{
    /// <summary>
    /// Api endpoint for managing 2fa authentication for user account
    /// </summary>
    /// <typeparam name="TUser"></typeparam>  
    public class AuthenticatorController<TUser, TKey> : Controller
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly UserManager<TUser> userManager;
        private readonly SignInManager<TUser> signInManager;
        private readonly UrlEncoder urlEncoder;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="urlEncoder"></param>
        public AuthenticatorController(UserManager<TUser> userManager,
            SignInManager<TUser> signInManager, UrlEncoder urlEncoder)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.urlEncoder = urlEncoder;
        }

        /// <summary>
        /// Gets whether authenticator is enabled for the user account
        /// </summary>
        /// <returns></returns>
        [HttpGet("isenabled")]
        public async Task<IActionResult> IsAuthenticatorEnabled()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse($"Unable to load user with ID '{userManager.GetUserId(User)}'."));
            }
            var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
            return Ok(isTwoFactorEnabled);
        }


        /// <summary>
        /// Get the details required for setting up authenticator for the user
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> GetUserAuthenticatorAndUri()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
               return NotFound(new NotFoundResponse($"Unable to load user with ID '{userManager.GetUserId(User)}'."));
            }

            var sharedKeyAndQrCode = await LoadSharedKeyAndQrCodeUriAsync(user);
          
            return Ok(new EnableAuthenticatorModel()
            {
                SharedKey = sharedKeyAndQrCode.Item1,
                AuthenticatorUri = sharedKeyAndQrCode.Item2
            });
        }

        /// <summary>
        /// Enable the authenticator by verifying the provided Code from authenticator app
        /// </summary>
        /// <returns></returns>
        [HttpPost("enable")]
        public async Task<IActionResult> EnableAuthenticator([FromBody] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new BadRequestResponse(new[] { "Code is required to enable authenticator." }));
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse($"Unable to load user with ID '{userManager.GetUserId(User)}'."));
            }

            // Strip spaces and hyphens
            var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(user,
                userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                return BadRequest(new BadRequestResponse(new[] { "Verification code is invalid." }));
            }

            await userManager.SetTwoFactorEnabledAsync(user, true);
            return Ok();          
        }

        /// <summary>
        /// Disable authenticator for the user account
        /// </summary>
        /// <returns></returns>
        [HttpPost("disable")]
        public async Task<IActionResult> DisableAuthenticator([FromBody] string code)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse($"Unable to load user with ID '{userManager.GetUserId(User)}'."));
            }

            // Strip spaces and hyphens
            var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(user,
               userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                return BadRequest(new BadRequestResponse(new[] { "Verification code is invalid." }));
            }

            var disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                return Problem(string.Join(Environment.NewLine, disable2faResult.GetErrors()));
            }
            return Ok();
        }

        /// <summary>
        /// Disable 2FA and reset authenticatory keys. User will need to setup authenticator again.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetAuthenticator([FromBody] string code)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse($"Unable to load user with ID '{userManager.GetUserId(User)}'."));
            }

            // Strip spaces and hyphens
            var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(user,
               userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                return BadRequest(new BadRequestResponse(new[] { "Verification code is invalid." }));
            }

            await userManager.SetTwoFactorEnabledAsync(user, false);
            await userManager.ResetAuthenticatorKeyAsync(user);
            var userId = await userManager.GetUserIdAsync(user);         
            await signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        /// <summary>
        /// Generate new recovery codes for the authenticator
        /// </summary>
        /// <returns></returns>
        [HttpGet("recoverycodes")]
        public async Task<IActionResult> GenerateRecoverCodes()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse($"Unable to load user with ID '{userManager.GetUserId(User)}'."));
            }

            var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactorEnabled)
            {
                return BadRequest(new BadRequestResponse(new[] { "2FA is not enabled for account." }));                
            }

            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return Ok(recoveryCodes);
        }

        /// <summary>
        /// Get the number of remaining recovery codes 
        /// </summary>
        /// <returns></returns>
        [HttpGet("recoverycodescount")]
        public async Task<IActionResult> CountActiveRecoveryCodes()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new NotFoundResponse($"Unable to load user with ID '{userManager.GetUserId(User)}'."));
            }
            int recoveryCodesCount = await userManager.CountRecoveryCodesAsync(user);
            return Ok(recoveryCodesCount);
        }

        private async Task<(string,string)> LoadSharedKeyAndQrCodeUriAsync(TUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            }
          
            var email = await userManager.GetEmailAsync(user);
            (string, string) sharedKeyAndQrCode =  (FormatKey(unformattedKey), GenerateQrCodeUri(email, unformattedKey));
            return sharedKeyAndQrCode;
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                AuthenticatorUriFormat,
                urlEncoder.Encode("Pixel-Identity"),
                urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
