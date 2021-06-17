using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Application
{
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
     
        async Task AddApplicationDetailsAsync()
        {
            var result = await Service.AddApplicationDescriptorAsync(application);
            if (result.IsSuccess)
            {
                SnackBar.Add("Added successfully.", Severity.Success, config =>
                {
                    config.ShowCloseIcon = true;                    
                });
                if(application.IsConfidentialClient)
                {
                    SnackBar.Add("Store client secret safely as it can't be viewed later.", Severity.Info, config =>
                    {
                        config.ShowCloseIcon = true;                        
                    });
                }              
                application = new ApplicationViewModel();
                return;
            }
            foreach (var error in result.ErrorMessages)
            {
              SnackBar.Add(error, Severity.Error, config =>
              {
                  config.ShowCloseIcon = true;
                  config.RequireInteraction = true;
              });
            }
        }
    }
}
