using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    /// <summary>
    /// Dialog to create a new user
    /// </summary>
    public partial class AddUserDialog : ComponentBase
    {
        RegisterViewModel model = new RegisterViewModel();

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Inject]
        IUsersService UsersService { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        /// <summary>
        /// Register a new user account with the application
        /// </summary>
        /// <returns></returns>
        public async Task AddUserAsync()
        {
            var result = await UsersService.CreateUserAsync(model);
            if (result.IsSuccess)
            {
                SnackBar.Add("Used created successfully.", Severity.Success);
                MudDialog.Close(true);
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
            MudDialog.Close(false);
        }
    }
}
