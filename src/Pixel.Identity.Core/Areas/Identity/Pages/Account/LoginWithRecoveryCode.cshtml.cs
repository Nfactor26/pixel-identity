using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(LoginWithRecoveryCodeModel<>))]
    public abstract class LoginWithRecoveryCodeModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
        }

        public virtual Task<IActionResult> OnGetAsync(string returnUrl = null) => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostAsync(string returnUrl = null) => throw new NotImplementedException();
    }

    public class LoginWithRecoveryCodeModel<TUser> : LoginWithRecoveryCodeModel where TUser : IdentityUser<Guid>, new()
    {
        private readonly SignInManager<TUser> signInManager;
        private readonly UserManager<TUser> userManager;

        public LoginWithRecoveryCodeModel(SignInManager<TUser> signInManager,
            UserManager<TUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public override async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            var userId = await userManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                //_logger.LogInformation(LoggerEventIds.UserLoginWithRecoveryCode, "User logged in with a recovery code.");
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                //_logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                //_logger.LogWarning(LoggerEventIds.InvalidRecoveryCode, "Invalid recovery code entered.");
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return Page();
            }
        }
    }
}
