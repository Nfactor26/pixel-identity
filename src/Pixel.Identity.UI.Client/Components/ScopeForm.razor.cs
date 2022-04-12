using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    /// <summary>
    /// Component use to create and update <see cref="OpenIddict.Abstractions.OpenIddictScopeDescriptor"/>
    /// </summary>
    public partial class ScopeForm : ComponentBase
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        [CascadingParameter]
        public ScopeViewModel Scope { get; set; }

        [Parameter]
        public IDialogService Dialog { get; set; }

        /// <summary>
        /// Shows a dialog to allow user to add a new resource to the scope.
        /// </summary>
        /// <returns></returns>
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
       
        /// <summary>
        /// Remove resource from the scope.
        /// </summary>
        /// <param name="scope"></param>
        void RemoveScopeResource(string scope)
        {
            if(Scope.Resources.Contains(scope))
            {
                Scope.Resources.Remove(scope);
            }
        }
    }
}
