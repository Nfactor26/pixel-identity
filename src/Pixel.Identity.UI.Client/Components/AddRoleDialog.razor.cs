using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class AddRoleDialog : ComponentBase
    {
        UserRoleViewModel selectedOption;

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IUserRolesService RolesService { get; set; }

        /// <summary>
        /// Close the dialog with selectedOption as dialog result
        /// </summary>
        void AddRole()
        {
            if (null != selectedOption)
            {
                MudDialog.Close(DialogResult.Ok<UserRoleViewModel>(selectedOption));
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
