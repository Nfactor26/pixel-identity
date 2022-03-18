using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class AddClaimDialog : ComponentBase
    {
        string error = null;
        ClaimViewModel model = new ();

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public IClaimsService Service { get; set; }

        [Parameter]
        public string Owner { get; set; }

        [Parameter]
        public IEnumerable<ClaimViewModel> ExistingClaims { get; set; }

        /// <summary>
        /// Close the dialog with model as dialog result
        /// </summary>
        async Task AddNewClaimAsync()
        {
            if (!ExistingClaims.Any(u => u.Type.Equals(model.Type) && u.Value.Equals(model.Value)))
            {
                //Don't try to add claim to role if role is not yet created.
                if (!string.IsNullOrEmpty(Owner))
                {
                    await Service.AddClaimAsync(Owner, model);
                }
                MudDialog.Close(DialogResult.Ok<ClaimViewModel>(model));
                return;
            }
            error = $"Claim with type {model.Type} already exists for role.";
        }

        /// <summary>
        /// Close the dialog without any result
        /// </summary>
        void Cancel() => MudDialog.Cancel();
    }
}
