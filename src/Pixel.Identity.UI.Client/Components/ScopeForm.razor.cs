using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class ScopeForm : ComponentBase
    {
        [CascadingParameter]
        public ScopeViewModel Scope { get; set; }

        [Parameter]
        public IDialogService Dialog { get; set; }

        async Task AddScopeResource()
        {
            var parameters = new DialogParameters();
            parameters.Add("ExistingResources", Scope.Resources);
            var dialog = Dialog.Show<AddResourceDialog>("Add Resource", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;
            if (!result.Cancelled && result.Data is string resource)
            {
                Scope.Resources.Add(resource);
            }
        }
       
        void RemoveScopeResource(string scope)
        {
            if(Scope.Resources.Contains(scope))
            {
                Scope.Resources.Remove(scope);
            }
        }
    }
}
