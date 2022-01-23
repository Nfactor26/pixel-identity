using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class AddResourceDialog : ComponentBase
    {
        string error = null;
        string resource = string.Empty;

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public IEnumerable<string> ExistingResources { get; set; }

        /// <summary>
        /// Close the dialog with resource as dialog result
        /// </summary>
        void AddNewResource()
        {
            if (!ExistingResources.Any(u => u.Equals(resource)))
            {
                MudDialog.Close(DialogResult.Ok<string>(resource));
                return;
            }
            error = "Resource is already added.";
            return;
        }

        /// <summary>
        /// Close the dialog without any result
        /// </summary>
        void Cancel() => MudDialog.Cancel();
    }
}
