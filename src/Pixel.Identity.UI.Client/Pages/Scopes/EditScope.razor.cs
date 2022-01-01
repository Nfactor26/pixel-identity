using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Scopes
{
    public partial class EditScope : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IScopeService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public string Id { get; set; }

        ScopeViewModel scope;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                scope = await Service.GetByIdAsync(Id);
                if(scope == null)
                {
                    SnackBar.Add("Failed to retrieve Scope.", Severity.Success, config =>
                    {
                        config.ShowCloseIcon = true;
                    });
                }
            }          
        }

        async Task UpdateScopeAsync()
        {
            var result = await Service.UpdateScopeAsync(scope);
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
