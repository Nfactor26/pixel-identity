using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;

namespace Pixel.Identity.UI.Client.Components
{
    /// <summary>
    /// ResetAuthenticator component allows user to reset the authenticator key.
    /// 2FA is disabled after this operation and user needs to re-enable 2FA and
    /// verify his authenticator app.
    /// </summary>
    public partial class ResetAuthenticator : ComponentBase
    {
        ResetAuthenticatorViewModel model = new();

        [Inject]
        public IAuthenticatorService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        /// <summary>
        /// Disable 2FA for user account and reset Authenticator key.      
        /// </summary>
        async void ResetAuthenticatorAsync()
        {
            var result = await Service.ResetAuthenticatorAsync(model.Code);
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
                "Authenticator is reset now. You need to re-configure authenticator again to enable 2FA!");
          
            Navigator.NavigateTo("account/authenticator/enable");
        }
    }
}
