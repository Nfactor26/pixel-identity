using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class RoleForm : ComponentBase
    {
        [CascadingParameter]
        public UserRoleViewModel Model { get; set; }

        [Parameter]
        public IDialogService Dialog { get; set; }

        async Task AddClaim()
        {
            var parameters = new DialogParameters();
            Console.WriteLine($"Model has {Model.Claims?.Count() ?? -1} claims");
            parameters.Add("ExistingClaims", Model.Claims);
            var dialog = Dialog.Show<AddClaimDialog>("Add Claim", parameters, new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, CloseButton = true });
            var result = await dialog.Result;
            if (!result.Cancelled && result.Data is ClaimViewModel claim)
            {
                Model.Claims.Add(claim);
            }
        }

        void RemoveClaim(ClaimViewModel claim)
        {
            if (Model.Claims.Contains(claim))
            {
                Model.Claims.Remove(claim);
            }
        }
    }
}
