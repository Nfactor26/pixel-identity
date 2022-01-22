using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.Models
{
    public class ResendEmailConfirmationModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
