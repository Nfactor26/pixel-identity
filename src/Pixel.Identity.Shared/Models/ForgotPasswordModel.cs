using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
