using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class RegisterViewModel
    {
        [Required]       
        public string Username { get; set; }

        [Required]
        [EmailAddress]      
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]   
        public string Password { get; set; }

        [DataType(DataType.Password)]       
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
