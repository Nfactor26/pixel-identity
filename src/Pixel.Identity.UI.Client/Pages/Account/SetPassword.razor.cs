using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Account
{
    /// <summary>
    /// Users who used external provider for login can setup a local account by adding a password.
    /// This will allow them to login using a local account when external login is not available due 
    /// to any reason.
    /// </summary>
    public partial class SetPassword : ComponentBase
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        SetPasswordModel model = new();              

        /// <summary>
        /// Set user password
        /// <returns></returns>
        async Task SetPasswordAsync()
        {
            var result = await AccountService.SetPasswordAsync(model);
            if (result.IsSuccess)
            {
                SnackBar.Add("Password added successfully.", Severity.Success);
                model = new();
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
