using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Pixel.Identity.Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [IdentityDefaultUI(typeof(ConfirmEmailModel<>))]
    public abstract class ConfirmEmailModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        public virtual Task<IActionResult> OnGetAsync(string userId, string code) => throw new NotImplementedException();
    }

    public class ConfirmEmailModel<TUser> : ConfirmEmailModel where TUser : IdentityUser<Guid>, new()
    {
        private readonly UserManager<TUser> _userManager;

        public ConfirmEmailModel(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return Page();
        }
    }
}
