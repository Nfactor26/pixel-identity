using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Components;
using Pixel.Identity.UI.Client.Services;
using System;
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
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IUsersService UsersService { get; set; }

        [Inject]
        public IUserRolesService RolesService { get; set; }

        UserDetailsViewModel user;       
           
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
        }

        async Task UpdateUserDetails()
        {          
            if(lockoutEndDate.HasValue && lockoutOffset.HasValue)
            {
                user.LockoutEnd = new DateTimeOffset(lockoutEndDate.Value, lockoutOffset.Value);
            }
            await UsersService.UpdateUserAsync(user);           
        }

        async Task DeleteRoleAsync(UserRoleViewModel roleToDelete)
        {
            var result = await RolesService.RemoveRolesFromUserAsync(user.UserName, new[] { roleToDelete });
            if(result.IsSuccess)
            {
                user.UserRoles.Remove(roleToDelete);
                SnackBar.Add($"Role was removed.", Severity.Success);
                return;
            }
            SnackBar.Add($"Error while removing role.{result}", Severity.Error);
        }

        async Task AddRoleAsync()
        {
            var parameters = new DialogParameters();           
            var dialog = Dialog.Show<AddRoleDialog>("Add New Role", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;          
            if (!result.Cancelled && result.Data is string role)
            {
                //if(user.UserRoles.Any(r => r.RoleName.Equals(role)))
                //{
                //    SnackBar.Add($"Role : {role} already mapped to user.", Severity.Info);
                //    return;
                //}
                var userRole = new UserRoleViewModel(role);
                var assignRoleResult =  await RolesService.AssignRolesToUserAsync(user.UserName, new[] { userRole });
                if (assignRoleResult.IsSuccess)
                {
                    user.UserRoles.Add(userRole);
                    SnackBar.Add($"Role was added.", Severity.Success);
                    return;
                }
                SnackBar.Add($"Error while adding role.{result}", Severity.Error);
            }
        }
    }
}
