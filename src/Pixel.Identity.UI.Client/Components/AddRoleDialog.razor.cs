using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class AddRoleDialog : ComponentBase
    {
        string error = null;
        UserRoleViewModel selectedOption;

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public IUserRolesService RolesService { get; set; }      

        [Parameter]
        public string Owner { get; set; }
       
        [Parameter]
        public IEnumerable<UserRoleViewModel> ExistingRoles { get; set; }

        /// <summary>
        /// Add role to user and close the dialog
        /// </summary>
        async Task AddRoleAsync()
        {
            if (null != selectedOption)
            {
                if (!ExistingRoles.Any(u => u.RoleName.Equals(selectedOption.RoleName)))
                {
                    var result = await RolesService.AssignRolesToUserAsync(Owner, new[] { selectedOption });
                    if (result.IsSuccess)
                    {
                        MudDialog.Close(DialogResult.Ok<UserRoleViewModel>(selectedOption));
                        return;
                    }
                    error = result.ToString();
                    return;                  
                }                
                error = $"{selectedOption.RoleName} role is already assigned to user.";
            }
        }

        /// <summary>
        /// Close the dialog without any result
        /// </summary>
        void Cancel() => MudDialog.Cancel();

        /// <summary>
        /// Get the filtered roles for auto complete text box as user types 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task<IEnumerable<UserRoleViewModel>> SearchRoles(string value)
        {
            // if text is null or empty, don't return values (drop-down will not open)
            if (string.IsNullOrEmpty(value))
                return null;
            var result = await RolesService.GetRolesAsync(new GetRolesRequest()
            {
                CurrentPage = 1,
                PageSize = 10,
                RoleFilter = value
            });
            return result.Items;
        }
    }
}
