using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.Models
{
    public class ChangeEmailModel
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
