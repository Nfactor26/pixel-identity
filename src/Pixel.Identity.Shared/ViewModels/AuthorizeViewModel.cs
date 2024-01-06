using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class AuthorizeViewModel
    {
        [Display(Name = "Application")]
        public string ApplicationName { get; set; } = string.Empty;

        [Display(Name = "Scope")]
        public string Scope { get; set; } = string.Empty;
    }
}
