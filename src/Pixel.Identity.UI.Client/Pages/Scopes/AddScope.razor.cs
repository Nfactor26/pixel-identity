using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Scopes
{
    public partial class AddScope : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IScopeService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }
      
        ScopeViewModel scope = new ScopeViewModel();

        async Task AddScopeAsync()
        {
            var result = await Service.AddScopeAsync(scope);
            if (result.IsSuccess)
            {
                SnackBar.Add("Added successfully.", Severity.Success, config =>
                {
                    config.ShowCloseIcon = true;
                });
                scope = new ScopeViewModel();
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
