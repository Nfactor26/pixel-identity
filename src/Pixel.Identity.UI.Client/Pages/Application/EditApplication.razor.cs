using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Application
{
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
       
        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                application = await Service.GetByClientIdAsync(clientId);                
            }
        }   
        
        async Task UpdateApplicationDetailsAsync()
        {
            var result = await Service.UpdateApplicationDescriptorAsync(application);
            if (result.IsSuccess)
            {
                SnackBar.Add("Updated successfully.", Severity.Success, config =>
                {
                    config.ShowCloseIcon = true;                    
                });
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
