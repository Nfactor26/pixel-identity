using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.ViewModels
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool IsLockedOut
        {
            get
            {
                return LockoutEnabled && LockoutEnd.HasValue;
            }
        }

        [Required]
        public List<UserRoleViewModel> UserRoles { get; set; } = new();

        [Required]
        public List<ClaimViewModel> UserClaims { get; set; } = new();
    }
}
