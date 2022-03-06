using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pixel.Identity.Core.Pages
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public virtual Task<IActionResult> OnGetAsync(string email, string returnUrl = null) => throw new NotImplementedException();
    }

    public class RegisterConfirmationModel<TUser> : RegisterConfirmationModel where TUser : class
    {
        private readonly UserManager<TUser> userManager;
        private readonly IEmailSender sender;

        public RegisterConfirmationModel(UserManager<TUser> userManager, IEmailSender sender)
        {
            this.userManager = userManager;
            this.sender = sender;
        }

        public override async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            // If the email sender is a no-op, display the confirm link in the page
            //DisplayConfirmAccountLink = sender is EmailSender;
            //if (DisplayConfirmAccountLink)
            //{
            //    var userId = await userManager.GetUserIdAsync(user);
            //    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //    EmailConfirmationUrl = Url.Page(
            //        "/Account/ConfirmEmail",
            //        pageHandler: null,
            //        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
            //        protocol: Request.Scheme);
            //}

            return Page();
        }
    }
}
