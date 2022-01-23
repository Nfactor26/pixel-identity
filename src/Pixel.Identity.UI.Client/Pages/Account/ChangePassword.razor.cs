using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Account
{
    /// <summary>
    /// Component for updating password
    /// </summary>
    public partial class ChangePassword : ComponentBase
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        ChangePasswordModel model = new();


        /// <summary>
        /// Change user password
        /// <returns></returns>
        async Task ChangePasswordAsync()
        {
            var result = await AccountService.ChangePasswordAsync(model);
            if (result.IsSuccess)
            {
                SnackBar.Add("Password update successfully.", Severity.Success);
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
