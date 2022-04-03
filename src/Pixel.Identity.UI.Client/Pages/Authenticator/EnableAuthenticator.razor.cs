using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
    public partial class EnableAuthenticator : ComponentBase, IAsyncDisposable
    {
        
        [Inject]
        public IAuthenticatorService Service { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        EnableAuthenticatorViewModel model = new();
        IJSObjectReference? module;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/Authenticator/EnableAuthenticator.razor.js");               
            }
            await GenerateQRCodeAsync();

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
            await DialogService.ShowMessageBox("Success", "2FA is enabled and your account is more secure now. ");
            Navigator.NavigateTo("account/authenticator/manage");
        }

        public async Task GenerateQRCodeAsync()
        {
            if(module is not null)
            {
                await module.InvokeVoidAsync("generateQrCode");
                return;
            }
           await Task.CompletedTask;
        }        

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }
    }
}
