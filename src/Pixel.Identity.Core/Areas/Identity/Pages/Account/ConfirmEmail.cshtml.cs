using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Pixel.Identity.Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(ConfirmEmailModel<,>))]
    public abstract class ConfirmEmailModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        public virtual Task<IActionResult> OnGetAsync(string userId, string code) => throw new NotImplementedException();
    }

    public class ConfirmEmailModel<TUser, TKey> : ConfirmEmailModel 
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> userManager;
        private readonly ILogger<ConfirmEmailModel> logger;

        public ConfirmEmailModel(UserManager<TUser> userManager, ILogger<ConfirmEmailModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public override async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await this.userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
            {
                StatusMessage = "Thank you for confirming your email.";
                logger.LogInformation("Email {emailId} was verified by user.", await userManager.GetEmailAsync(user));
            }
            else
            {
                StatusMessage = "Error confirming your email.";
                logger.LogWarning("Failed to verify email {emailId}", await userManager.GetEmailAsync(user));
                logger.LogError(String.Join(';', result.Errors));
            }
            return Page();
        }
    }
}
