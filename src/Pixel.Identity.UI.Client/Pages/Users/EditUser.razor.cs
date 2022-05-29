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
        public string userId { get; set; }

        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IUsersService UsersService { get; set; }

        [Inject]
        public IUserRolesService RolesService { get; set; }

        [Inject]
        public IUserClaimsService ClaimsService { get; set; }

        UserDetailsViewModel user;

        bool hasErrors = false;
      
        /// <summary>
        /// Retrieve user deatails when userName parameter is set
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            if(!string.IsNullOrEmpty(userId))
            {
                this.user = await GetUserDetailsAsync(userId);
            }           
        }

        /// <summary>
        /// Get user details for a given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        async Task<UserDetailsViewModel> GetUserDetailsAsync(string userId)
        {
            try
            {
                return await UsersService.GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                hasErrors = true;
                SnackBar.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
                return null;
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
                this.user = await GetUserDetailsAsync(userId);
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Temporariry lock user account for 90 days
        /// </summary>
        /// <returns></returns>
        async Task LockUserAccountAsync()
        {
            var result = await UsersService.LockUserAccountAsync(user);
            if (result.IsSuccess)
            {
                SnackBar.Add("User account locked successfully.", Severity.Success);               
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Unlock user account
        /// </summary>
        /// <returns></returns>
        async Task UnlockUserAccountAsync()
        {
            var result = await UsersService.UnlockUserAccountAsync(user);
            if (result.IsSuccess)
            {
                SnackBar.Add("User account unlocked successfully.", Severity.Success);
                this.user.LockoutEnd = null;
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
                SnackBar.Add("Role was successfully removed.", Severity.Success);
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
            parameters.Add("Owner", user.UserName);
            parameters.Add("ExistingRoles", user.UserRoles);
            parameters.Add("RolesService", RolesService);
            var dialog = Dialog.Show<AddRoleDialog>("Add New Role", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;         
            if (!result.Cancelled && result.Data is UserRoleViewModel role)
            {
                user.UserRoles.Add(role);
                SnackBar.Add("Role successfully assigned.", Severity.Success);
            }
        }

        /// <summary>
        /// Show a dialog to create and add a new claim to the role
        /// </summary>
        /// <returns></returns>
        async Task AddClaimAsync()
        {
            var parameters = new DialogParameters();
            parameters.Add("Owner", user.UserName);
            parameters.Add("ExistingClaims", user.UserClaims);
            parameters.Add("Service", ClaimsService);
            var dialog = Dialog.Show<AddClaimDialog>("Add Claim", parameters, new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, CloseButton = true });
            var result = await dialog.Result;
            if (!result.Cancelled && result.Data is ClaimViewModel claim)
            {
                user.UserClaims.Add(claim);
                SnackBar.Add("Claim was added.", Severity.Success);
            }
        }

        /// <summary>
        /// Delete an existing claim from the role
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        async Task RemoveClaimAsync(ClaimViewModel claim)
        {
            if (user.UserClaims.Contains(claim))
            {
                user.UserClaims.Remove(claim);
                var result = await ClaimsService.RemoveClaimAsync(user.UserName, claim);
                if (result.IsSuccess)
                {
                    SnackBar.Add($"Claim {claim.Type}:{claim.Value} was removed.", Severity.Success);
                }
                else
                {
                    SnackBar.Add($"Failed to delete claim {claim.Type}:{claim.Value}. {result}", Severity.Error);
                }
            }
        }

        /// <summary>
        /// Update details of existing claim
        /// </summary>
        /// <param name="original"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        async Task<bool> UpdateClaimAsync(ClaimViewModel original, ClaimViewModel modified)
        {
            var result = await ClaimsService.UpdateClaimAsync(user.UserName, original, modified);
            if (result.IsSuccess)
            {
                SnackBar.Add("Claim was updated.", Severity.Success);
                return true;
            }
            SnackBar.Add(result.ToString(), Severity.Error);
            return false;
        }
    }
}
