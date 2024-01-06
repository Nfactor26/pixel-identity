using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class EnableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Code is required to enable 2FA")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Key is required")]
        public string SharedKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Authenticator Uri is required")]
        public string AuthenticatorUri { get; set; } = string.Empty;
    }
}
