using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.Models
{
    public  class DeleteAccountModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
