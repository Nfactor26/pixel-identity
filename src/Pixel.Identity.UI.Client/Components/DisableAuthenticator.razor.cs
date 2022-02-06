using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;

namespace Pixel.Identity.UI.Client.Components
{
    /// <summary>
    /// DisableAuthenticator component allows disabling the 2FA authentication for user account.
    /// </summary>
    public partial class DisableAuthenticator : ComponentBase
    {
        DisableAuthenticatorViewModel model  = new();

        [Inject]
        public IAuthenticatorService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }


        /// <summary>
        /// Disable 2FA for user account
        /// </summary>
        async void DisableAuthenticatorAsync()
        {
            var result = await Service.DisableAuthenticatorAsync(model.Code);
            if (!result.IsSuccess)
            {
                SnackBar.Add(result.ToString(), Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
                return;
            }

            await DialogService.ShowMessageBox("Success",
              "2FA is disabled now. You should enable 2FA for a better security of your account.");

            Navigator.NavigateTo("account/authenticator/enable");
        }
    }
}
