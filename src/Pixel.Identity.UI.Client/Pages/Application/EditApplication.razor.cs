using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Application
{
    /// <summary>
    /// Component to edit application details
    /// </summary>
    public partial class EditApplication : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IApplicationService Service { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        [Parameter]
        public string clientId { get; set; }

        ApplicationViewModel application;

        bool hasErrors = false;
       
        /// <summary>
        /// Fetch application details when clientId is set
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            this.application = await GetApplicationDetailsAsync(this.clientId);
        }   
        
        /// <summary>
        /// Update application details
        /// </summary>
        /// <returns></returns>
        async Task UpdateApplicationDetailsAsync()
        {
            var result = await Service.UpdateApplicationDescriptorAsync(application);
            if (result.IsSuccess)
            {
                SnackBar.Add("Updated successfully.", Severity.Success);
                this.application = await GetApplicationDetailsAsync(this.clientId);             
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Retrieve application details given it's client id
        /// </summary>
        /// <param name="applicationClientId"></param>
        /// <returns></returns>
        private async Task<ApplicationViewModel> GetApplicationDetailsAsync(string applicationClientId)
        {
            if (!string.IsNullOrEmpty(applicationClientId))
            {
                try
                {
                    return await Service.GetByClientIdAsync(applicationClientId);                  
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    SnackBar.Add($"Failed to retrieve application data. {ex.Message}", Severity.Error);
                }
            }
            else
            {
                hasErrors = true;
                SnackBar.Add("No clientId specified.", Severity.Error);
            }
            return null;
        }
    }
}
