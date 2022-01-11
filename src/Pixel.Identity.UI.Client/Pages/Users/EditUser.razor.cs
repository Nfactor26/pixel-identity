using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Components;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Users
{
    /// <summary>
    /// Component for editing users
    /// </summary>
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

        /// <summary>
        /// Retrieve user deatails when userName parameter is set
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            if(!string.IsNullOrEmpty(userName))
            {
                try
                {
                    user = await UsersService.GetUserByNameAsync(userName);
                }
                catch (Exception ex)
                {
                    SnackBar.Add(ex.Message, Severity.Error, config =>
                    {
                        config.ShowCloseIcon = true;
                        config.RequireInteraction = true;
                    });
                }           
            }           
        }

        /// <summary>
        /// Update the details of user
        /// </summary>
        /// <returns></returns>
        async Task UpdateUserDetails()
        {
            var result = await UsersService.UpdateUserAsync(user);
            if (result.IsSuccess)
            {
                SnackBar.Add("User details updated successfully.", Severity.Success);
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Remove a assigned role from user
        /// </summary>
        /// <param name="roleToDelete"></param>
        /// <returns></returns>
        async Task DeleteRoleAsync(UserRoleViewModel roleToDelete)
        {
            var result = await RolesService.RemoveRolesFromUserAsync(user.UserName, new[] { roleToDelete });
            if(result.IsSuccess)
            {
                user.UserRoles.Remove(roleToDelete);
                SnackBar.Add($"Role was successfully removed.", Severity.Success);
                return;
            }
            SnackBar.Add($"Error while removing role.{result}", Severity.Error);
        }

        /// <summary>
        /// Assign a new role to user
        /// </summary>
        /// <returns></returns>
        async Task AddRoleAsync()
        {
            var parameters = new DialogParameters();           
            var dialog = Dialog.Show<AddRoleDialog>("Add New Role", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;          
            if (!result.Cancelled && result.Data is string role)
            {
                var userRole = new UserRoleViewModel(role);
                var assignRoleResult =  await RolesService.AssignRolesToUserAsync(user.UserName, new[] { userRole });
                if (assignRoleResult.IsSuccess)
                {
                    user.UserRoles.Add(userRole);
                    SnackBar.Add($"Role successfully assigned.", Severity.Success);
                    return;
                }
                SnackBar.Add($"Error while assigning role.{result}", Severity.Error);
            }
        }
    }
}
