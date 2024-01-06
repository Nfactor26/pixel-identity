using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class ScopeViewModel
    {
        /// <summary>
        /// Identifier of the scope
        /// </summary> 
        [Required(AllowEmptyStrings = true)]
        public string Id { get; set; } = string.Empty;

        private string name = string.Empty;
        /// <summary>
        /// Name of the scope
        /// </summary>
        [Required]
        public string Name
        {
            get => name;          
            set => name = value;
        }

        /// <summary>
        /// Display name of the scope
        /// </summary>
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Description of the scope
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Resources associated with the scope.
        /// Resources are added on claims prinicipal
        /// </summary>
        public List<string> Resources { get; set; } = [];

    }
}
