using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Components;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Roles
{
    /// <summary>
    /// Component to edit role
    /// </summary>
    public partial class EditRole : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IUserRolesService UserRoleService { get; set; }

        [Inject]
        public IRoleClaimsService RoleClaimsService { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public string Name { get; set; }

        UserRoleViewModel model;

        bool canEditRoleName = false;

        bool hasErrors = false;
       
        /// <summary>
        /// Retrieve role details when role name parameter is set
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                try
                {
                    this.model = await UserRoleService.GetRoleByNameAsync(Name);                 
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    SnackBar.Add($"Failed to retrieve role data for role : {Name}. {ex.Message}", Severity.Error);
                }                
            }
            else
            {
                hasErrors = true;
                SnackBar.Add("No role specified to edit.", Severity.Error);
            }
        }

        /// <summary>
        /// Toggle the state of canEditRoleName to enable or disable editing role name on ui
        /// </summary>
        void ToggleEditRoleName()
        {
            canEditRoleName = !canEditRoleName;
        }

        /// <summary>
        /// Update role name to a new value
        /// </summary>
        /// <returns></returns>
        async Task UpdateRoleNameAsync()
        {
            var result = await UserRoleService.UpdateRoleNameAsync(new() { RoleId = model.RoleId, NewName = model.RoleName });
            if (result.IsSuccess)
            {
                ToggleEditRoleName();
                SnackBar.Add("Updated successfully.", Severity.Success);
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Show a dialog to create and add a new claim to the role
        /// </summary>
        /// <returns></returns>
        async Task AddClaimAsync()
        {
            var parameters = new DialogParameters();
            parameters.Add("Owner", model.RoleName);
            parameters.Add("ExistingClaims", model.Claims);
            parameters.Add("Service", RoleClaimsService);
            var dialog = Dialog.Show<AddClaimDialog>("Add Claim", parameters, new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, CloseButton = true });
            var result = await dialog.Result;
            if (!result.Cancelled && result.Data is ClaimViewModel claim)
            {
                model.Claims.Add(claim);
                SnackBar.Add($"Claim was added.", Severity.Success);
            }
        }

        /// <summary>
        /// Delete an existing claim from the role
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        async Task RemoveClaimAsync(ClaimViewModel claim)
        {
            if (model.Claims.Contains(claim))
            {               
                var result = await RoleClaimsService.RemoveClaimAsync(model.RoleName, claim);
                if (result.IsSuccess)
                {
                    model.Claims.Remove(claim);
                    SnackBar.Add($"Claim {claim.Type}:{claim.Value} was removed.", Severity.Success);
                    return;
                }
                SnackBar.Add(result.ToString(), Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });              
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
            var result = await RoleClaimsService.UpdateClaimAsync(model.RoleName, original, modified);
            if (result.IsSuccess)
            {
                SnackBar.Add($"Claim was updated.", Severity.Success);
                return true;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
            return false;
        }
    }
}
