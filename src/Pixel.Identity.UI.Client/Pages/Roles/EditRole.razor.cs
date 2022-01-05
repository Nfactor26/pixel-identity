using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
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
        public IUserRolesService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public string Name { get; set; }

        UserRoleViewModel model = new UserRoleViewModel() { RoleName = String.Empty };

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                try
                {
                    this.model = await Service.GetRoleByNameAsync(Name);
                }
                catch (Exception ex)
                {
                    SnackBar.Add($"Failed to retrieve role data for role : {Name}. {ex.Message}", Severity.Error);
                }                
            }
            else
            {
                SnackBar.Add("No role specified to edit.", Severity.Error);
            }
        }
    }
}
