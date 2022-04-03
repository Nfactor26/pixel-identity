using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class UserRoleViewModel
    {
        [Required(AllowEmptyStrings = true)]
        public string RoleId { get; set; }

        [Required]       
        public string RoleName { get; set; }

        [Required]       
        public List<ClaimViewModel> Claims { get; set; } = new List<ClaimViewModel>();

        public bool Exists => !string.IsNullOrEmpty(RoleId);

        public UserRoleViewModel()
        {

        }

        public UserRoleViewModel(string roleName)
        {
            this.RoleId = string.Empty;
            this.RoleName = roleName;
        }

        public UserRoleViewModel(string roleId, string roleName) : this(roleName)
        {
            this.RoleId = roleId;         
        }
    }
}
