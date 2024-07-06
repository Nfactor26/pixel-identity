using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class AddScopeDialog : ComponentBase
    {
        ScopeViewModel selectedOption;

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IScopeService ScopesService { get; set; }

        /// <summary>
        /// Close the dialog with selectedOption as dialog result
        /// </summary>
        void AddScope()
        {
            if (null != selectedOption)
            {
                MudDialog.Close(DialogResult.Ok<ScopeViewModel>(selectedOption));
            }
        }

        /// <summary>
        /// Close the dialog without any result
        /// </summary>
        void Cancel() => MudDialog.Cancel();

        /// <summary>
        /// Get the filtered scopes for auto complete text box as user types 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        async Task<IEnumerable<ScopeViewModel>> SearchScopes(string value, CancellationToken ct)
        {
            // if text is null or empty, don't return values (drop-down will not open)
            if (string.IsNullOrEmpty(value))
                return null;
            var result = await ScopesService.GetScopesAsync(new GetScopesRequest()
            {
                CurrentPage = 1,
                PageSize = 10,
                ScopesFilter = value
            });
            return result.Items;
        }
    }
}
