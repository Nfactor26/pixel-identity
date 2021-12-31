using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Components;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Users
{
    public partial class EditUser : ComponentBase
    {
        [Parameter]
        public string userName { get; set; }

        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IUsersService UsersService { get; set; }

        [Inject]
        public IUserRolesService RolesService { get; set; }

        UserDetailsViewModel user;
        IEnumerable<UserRoleViewModel> availableRoles = Enumerable.Empty<UserRoleViewModel>();
        string roleToAdd;
      
        bool updated;

        DateTime? lockoutEndDate;
        TimeSpan? lockoutOffset;

        protected override async Task OnParametersSetAsync()
        {
            if(!string.IsNullOrEmpty(userName))
            {
                user = await UsersService.GetUserByNameAsync(userName);
                if (user.LockoutEnd.HasValue)
                {
                    lockoutEndDate = user.LockoutEnd.Value.UtcDateTime;
                    lockoutOffset = user.LockoutEnd.Value.Offset;
                }
            }

            availableRoles = await RolesService.GetAll();
        }

        async Task UpdateUserDetails()
        {
            updated = false;
            if(lockoutEndDate.HasValue && lockoutOffset.HasValue)
            {
                user.LockoutEnd = new DateTimeOffset(lockoutEndDate.Value, lockoutOffset.Value);
            }
            updated = await UsersService.UpdateUserAsync(user);           
        }

        async Task DeleteRoleAsync(UserRoleViewModel roleToDelete)
        {
            var result = await RolesService.RemoveRolesFromUserAsync(user.UserName, new[] { roleToDelete });
            if(result)
            {
                user.UserRoles.Remove(roleToDelete);
            }
        }

        async Task AddRoleAsync()
        {
            var parameters = new DialogParameters();
            parameters.Add("AvailableRoles", this.availableRoles.Except(user.UserRoles));
            var dialog = Dialog.Show<AddRoleDialog>("Add New Role", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;          
            if (!result.Cancelled && result.Data is string role)
            {
                var roleToAdd = availableRoles.FirstOrDefault(r => r.RoleName.Equals(role));
                if (roleToAdd != null)
                {
                    var success = await RolesService.AssignRolesToUserAsync(user.UserName, new[] { roleToAdd });
                    if (success)
                    {
                        user.UserRoles.Add(roleToAdd);
                    }
                }
            }
        }
    }
}
