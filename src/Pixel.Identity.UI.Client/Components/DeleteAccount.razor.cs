using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    /// <summary>
    /// Component to facilitate user account deletion
    /// </summary>
    public partial class DeleteAccount : ComponentBase
    {
        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        DeleteAccountModel model = new ();

        /// <summary>
        /// Permantently delete user account
        /// </summary>
        /// <returns></returns>
        async Task DeleteAccountAsync()
        {
            var result = await AccountService.DeleteAccountAsync(model);
            if (result.IsSuccess)
            {
                Navigator.NavigateToLogout("authentication/register");
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
