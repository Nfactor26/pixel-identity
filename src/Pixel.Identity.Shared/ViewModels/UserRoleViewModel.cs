using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class UserRoleViewModel
    {
        [Required]     
        public Guid RoleId { get; set; }

        [Required]       
        public string RoleName { get; set; }

        [Required]       
        public List<ClaimViewModel> Claims { get; set; } = new List<ClaimViewModel>();

        public bool Exists => !Guid.Empty.Equals(RoleId);

        public UserRoleViewModel()
        {

        }

        public UserRoleViewModel(string roleName)
        {          
            this.RoleName = roleName;
        }

        public UserRoleViewModel(Guid roleId, string roleName) : this(roleName)
        {
            this.RoleId = roleId;         
        }
    }
}
