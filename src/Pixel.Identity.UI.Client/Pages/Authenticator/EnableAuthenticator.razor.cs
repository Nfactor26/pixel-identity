using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Authenticator
{
    /// <summary>
    /// Component for setting up the authenticator for 2FA
    /// </summary>
    public partial class EnableAuthenticator : ComponentBase
    {
        
        [Inject]
        public IAuthenticatorService Service { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        EnableAuthenticatorViewModel model = new();         

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await Service.GetAuthenticatorSetupConfigurationAsync();         
                model.SharedKey = result.SharedKey;
                model.AuthenticatorUri = result.AuthenticatorUri;
            }
            catch (Exception ex)
            {
                SnackBar.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Enable the authenticator once user has completed the required setup steps
        /// </summary>
        /// <returns></returns>
        async Task EnableAuthenticatorAsync()
        {
            var result = await Service.EnableAuthenticatorAsync(model.Code);
            if (!result.IsSuccess)
            {
                SnackBar.Add(result.ToString(), Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
                return;
            }
            Navigator.NavigateTo("account/authenticator/manage");
        }
    }
}
