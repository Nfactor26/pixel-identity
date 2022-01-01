using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Pixel.Identity.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    [AutoMap(typeof(ApplicationRole))]
    public class UserRoleViewModel
    {
        [Required]
        [SourceMember(nameof(ApplicationRole.Id))]
        public Guid RoleId { get; set; }

        [Required]
        [SourceMember(nameof(ApplicationRole.Name))]
        public string RoleName { get; set; }

        [Required]
        [SourceMember(nameof(ApplicationRole.Claims))]
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
