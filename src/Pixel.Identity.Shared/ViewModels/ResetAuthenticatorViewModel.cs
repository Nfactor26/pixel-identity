using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class ResetAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Code is required to reset Authenticator")]
        public string Code { get; set; }
    }
}
