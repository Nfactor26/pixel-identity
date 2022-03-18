using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(LoginWith2faModel<>))]
    public abstract class LoginWith2faModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }


            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public virtual Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null) => throw new NotImplementedException();

        public virtual Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null) => throw new NotImplementedException();
    }

    public class LoginWith2faModel<TUser> : LoginWith2faModel where TUser : IdentityUser<Guid>, new()
    {
        private readonly SignInManager<TUser> signInManager;
        private readonly UserManager<TUser> userManager;
        private readonly ILogger<LoginWith2faModel> logger;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        public LoginWith2faModel(SignInManager<TUser> signInManager, UserManager<TUser> userManager,
            ILogger<LoginWith2faModel> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        public override async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            var userId = await userManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                logger.LogInformation("User logged in with 2fa.");
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                logger.LogWarning("Invalid authenticator code entered by user");
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return Page();
            }
        }
    }
}
