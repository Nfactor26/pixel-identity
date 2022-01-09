using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Roles
{
    /// <summary>
    /// Component to add new roles
    /// </summary>
    public partial class AddRole : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IUserRolesService UserRolesService { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }
      
        UserRoleViewModel model = new UserRoleViewModel(string.Empty);

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <returns></returns>
        async Task AddRoleAsync()
        {
            if (!string.IsNullOrEmpty(model.RoleName))
            {
                try
                {
                    var result = await UserRolesService.CreateRoleAsync(model);
                    if(result.IsSuccess)
                    {
                        SnackBar.Add("Added successfully.", Severity.Success);
                        model = new UserRoleViewModel(string.Empty);
                        return;
                    }
                    SnackBar.Add(result.ToString(), Severity.Error);
                }
                catch (Exception ex)
                {
                    SnackBar.Add(ex.Message, Severity.Error);
                }                  
            }
        }
    }
}
