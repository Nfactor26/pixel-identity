using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Roles
{
    /// <summary>
    /// Add role view allows user to create a new Asp.Net Identity Role/>
    /// </summary>
    public partial class AddRole : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IUserRolesService UserRolesService { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        UserRoleViewModel model = new UserRoleViewModel(string.Empty);

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <returns></returns>
        async Task AddRoleAsync()
        {
            var result = await UserRolesService.CreateRoleAsync(model);
            if (result.IsSuccess)
            {
                SnackBar.Add("Added successfully.", Severity.Success);
                Navigator.NavigateTo($"roles/list");
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }
    }
}
