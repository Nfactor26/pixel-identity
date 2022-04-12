using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Scopes
{
    /// <summary>
    /// Edit scope view allows authorized user to edit an existing <see cref="OpenIddict.Abstractions.OpenIddictScopeDescriptor"/>
    /// </summary>
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

        /// <summary>
        /// Retrieve the Scope details when Id parameter is set
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    this.scope = await Service.GetByIdAsync(Id);
                    return;
                }
            }
            catch (Exception ex)
            {
                SnackBar.Add($"Failed to retrieve scope. {ex.Message}", Severity.Error);
            }

            SnackBar.Add("No role specified to edit.", Severity.Error);
        }

        /// <summary>
        /// Update the details of scope 
        /// </summary>
        /// <returns></returns>
        async Task UpdateScopeAsync()
        {
            var result = await Service.UpdateScopeAsync(scope);
            if (result.IsSuccess)
            {
                this.scope = await Service.GetByIdAsync(Id);
                SnackBar.Add("Updated successfully.", Severity.Success);              
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }
    }
}
