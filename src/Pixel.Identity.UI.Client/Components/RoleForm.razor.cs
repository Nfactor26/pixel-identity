using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class RoleForm : ComponentBase
    {
        [CascadingParameter]
        public UserRoleViewModel Model { get; set; }

        [Parameter]
        public IDialogService Dialog { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IUserRolesService Service { get; set; }

        /// <summary>
        /// Show a dialog to create and add a new claim to the role
        /// </summary>
        /// <returns></returns>
        async Task AddClaimAsync()
        {
            var parameters = new DialogParameters();
            if(Model.Exists)
            {
                parameters.Add("RoleName", Model.RoleName);
            }
            parameters.Add("ExistingClaims", Model.Claims);
            var dialog = Dialog.Show<AddClaimDialog>("Add Claim", parameters, new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, CloseButton = true });
            var result = await dialog.Result;
            if (!result.Cancelled && result.Data is ClaimViewModel claim)
            {
                Model.Claims.Add(claim);
                if (Model.Exists)
                {
                    SnackBar.Add($"Claim was added.", Severity.Success);
                }
            }
        }

        /// <summary>
        /// Delete an existing claim from the role
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        async Task RemoveClaimAsync(ClaimViewModel claim)
        {
            if (Model.Claims.Contains(claim))
            {
                Model.Claims.Remove(claim);
                if(Model.Exists)
                {
                   var result =  await Service.removeClaimFromRoleAsync(Model.RoleName, claim);
                    if (result.IsSuccess)
                    {
                        SnackBar.Add($"Claim was removed.", Severity.Success);
                    }
                    else
                    {
                        SnackBar.Add($"Failed to delete claim. {result}", Severity.Error);           
                    }                     
                }
            }
        }
    }
}
