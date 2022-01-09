using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Application
{
    /// <summary>
    /// Component to add new application details
    /// </summary>
    public partial class AddApplication : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IApplicationService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        ApplicationViewModel application = new ApplicationViewModel();
     
        /// <summary>
        /// Add new application details
        /// </summary>
        /// <returns></returns>
        async Task AddApplicationDetailsAsync()
        {
            var result = await Service.AddApplicationDescriptorAsync(application);
            if (result.IsSuccess)
            {
                SnackBar.Add("Added successfully.", Severity.Success);
                if(application.IsConfidentialClient)
                {
                    SnackBar.Add("Store client secret safely as it can't be viewed later.", Severity.Info);
                }              
                application = new ApplicationViewModel();
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
